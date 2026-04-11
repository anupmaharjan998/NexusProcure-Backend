using Hangfire;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Inventory;

public class PurchaseOrderReceiptService : IPurchaseOrderReceiptService
{
    private readonly NexusProcureDbContext _context;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public PurchaseOrderReceiptService(
        NexusProcureDbContext context,
        IBackgroundJobClient backgroundJobClient)
    {
        _context = context;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<GoodsReceiptResultDto> ReceivePurchaseOrderAsync(
        ReceivePurchaseOrderDto dto,
        Guid receivedBy)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Items)
            .FirstOrDefaultAsync(po => po.Id == dto.PurchaseOrderId);

        if (purchaseOrder == null)
            throw new Exception("Purchase order not found.");

        if (dto.Items == null || dto.Items.Count == 0)
            throw new Exception("At least one received item is required.");

        var goodsReceipt = new GoodsReceipt
        {
            Id = Guid.NewGuid(),
            PurchaseOrderId = purchaseOrder.Id,
            ReceivedDate = dto.ReceivedDate ?? DateTime.UtcNow,
            ReceivedById = receivedBy,
            Notes = dto.Notes,
            InventoryProcessingStatus = InventoryProcessingStatus.Pending,
            Items = new List<GoodsReceiptItem>()
        };

        foreach (var itemDto in dto.Items)
        {
            var poItem = purchaseOrder.Items.FirstOrDefault(x => x.Id == itemDto.PurchaseOrderItemId);
            if (poItem == null)
                throw new Exception($"Purchase order item {itemDto.PurchaseOrderItemId} not found.");

            if (itemDto.QuantityReceived <= 0)
                throw new Exception($"Quantity received must be greater than zero for item {poItem.ItemName}.");

            var alreadyReceived = await _context.GoodsReceiptItems
                .Where(x => x.PurchaseOrderItemId == poItem.Id)
                .SumAsync(x => (int?)x.QuantityReceived) ?? 0;

            var remaining = poItem.Quantity - alreadyReceived;

            if (itemDto.QuantityReceived > remaining)
                throw new Exception(
                    $"Cannot receive {itemDto.QuantityReceived} units for {poItem.ItemName}. Remaining quantity is {remaining}."
                );

            goodsReceipt.Items.Add(new GoodsReceiptItem
            {
                Id = Guid.NewGuid(),
                PurchaseOrderItemId = poItem.Id,
                QuantityReceived = itemDto.QuantityReceived,
                Location = itemDto.Location,
                Condition = itemDto.Condition,
                Notes = itemDto.Notes,
                InventoryInserted = false
            });
        }

        _context.GoodsReceipts.Add(goodsReceipt);
        await _context.SaveChangesAsync();

        await UpdateDeliveryStatusAsync(purchaseOrder.Id);

        _backgroundJobClient.Enqueue<IInventoryReceiptJob>(
            job => job.InsertReceivedItemsIntoInventoryAsync(goodsReceipt.Id));

        var refreshedPo = await _context.PurchaseOrders.FirstAsync(po => po.Id == purchaseOrder.Id);
        var savedPo = await _context.PurchaseOrders.FirstAsync(po => po.Id == purchaseOrder.Id);

        return new GoodsReceiptResultDto
        {
            GoodsReceiptId = goodsReceipt.Id,
            PurchaseOrderId = purchaseOrder.Id,
            PurchaseOrderNumber = savedPo.PurchaseOrderNumber,
            DeliveryStatus = refreshedPo.DeliveryStatus.ToString(),
            InventoryProcessingStatus = goodsReceipt.InventoryProcessingStatus.ToString(),
            Message = "Goods receipt created successfully. Inventory insertion queued."
        };
    }
    
    
    public async Task<IEnumerable<PurchaseOrderDeliveryListDto>> GetReceivingDeliveriesAsync(
        PurchaseOrderDeliveryQueryDto query)
    {
        var targetDate = (query.Date ?? DateTime.UtcNow.Date).Date;

        var purchaseOrders = await _context.PurchaseOrders
            .Include(po => po.Vendor)
            .Include(po => po.Items)
            .Where(po =>
                po.DeliveryDate.HasValue &&
                po.DeliveryDate.Value.Date == targetDate ||
                po.DeliveryStatus == DeliveryStatus.PartiallyReceived)
            .OrderBy(po => po.DeliveryDate)
            .ToListAsync();

        var purchaseOrderIds = purchaseOrders.Select(po => po.Id).ToList();
        var purchaseOrderItemIds = purchaseOrders
            .SelectMany(po => po.Items)
            .Select(i => i.Id)
            .ToList();

        var receivedLookup = await _context.GoodsReceiptItems
            .Where(gri => purchaseOrderItemIds.Contains(gri.PurchaseOrderItemId))
            .GroupBy(gri => gri.PurchaseOrderItemId)
            .Select(g => new
            {
                PurchaseOrderItemId = g.Key,
                ReceivedQty = g.Sum(x => x.QuantityReceived)
            })
            .ToDictionaryAsync(x => x.PurchaseOrderItemId, x => x.ReceivedQty);

        var result = purchaseOrders.Select(po =>
        {
            var items = po.Items.Select(item =>
            {
                var receivedQty = receivedLookup.TryGetValue(item.Id, out var qty) ? qty : 0;
                var remainingQty = Math.Max(0, item.Quantity - receivedQty);

                return new PurchaseOrderDeliveryItemDto
                {
                    PurchaseOrderItemId = item.Id,
                    ItemName = item.ItemName,
                    SKU = null,
                    OrderedQty = item.Quantity,
                    ReceivedQty = receivedQty,
                    RemainingQty = remainingQty,
                    UnitPrice = item.UnitPrice
                };
            }).ToList();

            return new PurchaseOrderDeliveryListDto
            {
                Id = po.Id,
                PurchaseOrderNumber = po.PurchaseOrderNumber,
                VendorName = po.Vendor.VendorName,
                ExpectedDate = po.DeliveryDate,
                Status = po.DeliveryStatus.ToString(),
                Location = "Main Receiving Bay",
                TotalItems = items.Count,
                Items = items
            };
        });

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            result = result.Where(x =>
                x.PurchaseOrderNumber.ToLower().Contains(search) ||
                x.VendorName.ToLower().Contains(search) ||
                x.Location.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            result = result.Where(x => x.Status.Equals(query.Status, StringComparison.OrdinalIgnoreCase));
        }

        return result.ToList();
    }

    public async Task<PurchaseOrderDeliveryListDto?> GetReceivingDeliveryByPurchaseOrderIdAsync(Guid purchaseOrderId)
    {
        var po = await _context.PurchaseOrders
            .Include(x => x.Vendor)
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == purchaseOrderId);

        if (po == null) return null;

        var poItemIds = po.Items.Select(i => i.Id).ToList();

        var receivedLookup = await _context.GoodsReceiptItems
            .Where(gri => poItemIds.Contains(gri.PurchaseOrderItemId))
            .GroupBy(gri => gri.PurchaseOrderItemId)
            .Select(g => new
            {
                PurchaseOrderItemId = g.Key,
                ReceivedQty = g.Sum(x => x.QuantityReceived)
            })
            .ToDictionaryAsync(x => x.PurchaseOrderItemId, x => x.ReceivedQty);

        var items = po.Items.Select(item =>
        {
            var receivedQty = receivedLookup.TryGetValue(item.Id, out var qty) ? qty : 0;
            var remainingQty = Math.Max(0, item.Quantity - receivedQty);

            return new PurchaseOrderDeliveryItemDto
            {
                PurchaseOrderItemId = item.Id,
                ItemName = item.ItemName,
                SKU = null,
                OrderedQty = item.Quantity,
                ReceivedQty = receivedQty,
                RemainingQty = remainingQty,
                UnitPrice = item.UnitPrice
            };
        }).ToList();

        return new PurchaseOrderDeliveryListDto
        {
            Id = po.Id,
            PurchaseOrderNumber = po.PurchaseOrderNumber,
            VendorName = po.Vendor.VendorName,
            ExpectedDate = po.DeliveryDate,
            Status = po.DeliveryStatus.ToString(),
            Location = "Main Receiving Bay",
            TotalItems = items.Count,
            Items = items
        };
    }

    private async Task UpdateDeliveryStatusAsync(Guid purchaseOrderId)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Items)
            .FirstAsync(po => po.Id == purchaseOrderId);

        var poItemIds = purchaseOrder.Items.Select(x => x.Id).ToList();

        var receiptTotals = await _context.GoodsReceiptItems
            .Where(x => poItemIds.Contains(x.PurchaseOrderItemId))
            .GroupBy(x => x.PurchaseOrderItemId)
            .Select(g => new
            {
                PurchaseOrderItemId = g.Key,
                Received = g.Sum(x => x.QuantityReceived)
            })
            .ToListAsync();

        var totalOrdered = purchaseOrder.Items.Sum(x => x.Quantity);
        var totalReceived = receiptTotals.Sum(x => x.Received);

        if (totalReceived <= 0)
            purchaseOrder.DeliveryStatus = DeliveryStatus.Pending;
        else if (totalReceived < totalOrdered)
            purchaseOrder.DeliveryStatus = DeliveryStatus.PartiallyReceived;
        else
            purchaseOrder.DeliveryStatus = DeliveryStatus.Received;

        await _context.SaveChangesAsync();
    }
}