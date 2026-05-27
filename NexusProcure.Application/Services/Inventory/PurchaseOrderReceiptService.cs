using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;
using InventoryItem = NexusProcure.Core.Entities.Inventory.InventoryItem;

namespace NexusProcure.Application.Services.Inventory;

public class PurchaseOrderReceiptService : IPurchaseOrderReceiptService
{
    private readonly NexusProcureDbContext _context;

    public PurchaseOrderReceiptService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<GoodsReceiptResultDto> ReceivePurchaseOrderAsync(
        ReceivePurchaseOrderDto dto,
        Guid receivedBy)
    {
        if (dto.Items == null || dto.Items.Count == 0)
            throw new Exception("At least one received item is required.");

        await using var tx = await _context.Database.BeginTransactionAsync();

        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Items)
                .ThenInclude(i => i.InventoryCategory)
            .FirstOrDefaultAsync(po => po.Id == dto.PurchaseOrderId);

        if (purchaseOrder == null)
            throw new Exception("Purchase order not found.");

        if (purchaseOrder.DeliveryStatus == DeliveryStatus.Received)
            throw new Exception("This purchase order has already been fully received.");

        var poItemIds = purchaseOrder.Items
            .Select(x => x.Id)
            .ToList();

        var receivedLookup = await _context.GoodsReceiptItems
            .Where(x => poItemIds.Contains(x.PurchaseOrderItemId))
            .GroupBy(x => x.PurchaseOrderItemId)
            .Select(g => new
            {
                PurchaseOrderItemId = g.Key,
                ReceivedQty = g.Sum(x => x.QuantityReceived)
            })
            .ToDictionaryAsync(x => x.PurchaseOrderItemId, x => x.ReceivedQty);

        var goodsReceipt = new GoodsReceipt
        {
            Id = Guid.NewGuid(),
            PurchaseOrderId = purchaseOrder.Id,
            ReceivedDate = ToUtc(dto.ReceivedDate) ?? DateTime.UtcNow,
            ReceivedById = receivedBy,
            Notes = dto.Notes,
            InventoryProcessingStatus = InventoryProcessingStatus.Pending,
            Items = new List<GoodsReceiptItem>()
        };

        foreach (var itemDto in dto.Items)
        {
            var poItem = purchaseOrder.Items
                .FirstOrDefault(x => x.Id == itemDto.PurchaseOrderItemId);

            if (poItem == null)
                throw new Exception($"Purchase order item {itemDto.PurchaseOrderItemId} not found.");

            if (itemDto.QuantityReceived <= 0)
                throw new Exception($"Quantity received must be greater than zero for item {poItem.ItemName}.");

            var alreadyReceived = receivedLookup.TryGetValue(poItem.Id, out var receivedQty)
                ? receivedQty
                : 0;

            var remaining = poItem.Quantity - alreadyReceived;

            if (remaining <= 0)
                throw new Exception($"{poItem.ItemName} has already been fully received.");

            if (itemDto.QuantityReceived > remaining)
            {
                throw new Exception(
                    $"Cannot receive {itemDto.QuantityReceived} units for {poItem.ItemName}. " +
                    $"Remaining quantity is {remaining}."
                );
            }

            if (poItem.InventoryCategoryId == null)
                throw new Exception($"Inventory category missing for {poItem.ItemName}.");

            var category = poItem.InventoryCategory;

            if (category == null)
            {
                category = await _context.InventoryCategories
                    .FirstOrDefaultAsync(x => x.Id == poItem.InventoryCategoryId.Value);
            }

            if (category == null)
                throw new Exception($"Inventory category not found for {poItem.ItemName}.");

            var location = string.IsNullOrWhiteSpace(itemDto.Location)
                ? "Inventory"
                : itemDto.Location.Trim();

            var condition = string.IsNullOrWhiteSpace(itemDto.Condition)
                ? "Good"
                : itemDto.Condition.Trim();

            var stock = await _context.InventoryStocks
                .FirstOrDefaultAsync(x =>
                    x.Name == poItem.ItemName &&
                    x.CategoryId == category.Id);

            if (stock == null)
            {
                var stockSku = await GenerateUniqueStockSkuAsync(poItem.ItemName, category.Id);

                stock = new InventoryStock
                {
                    Id = Guid.NewGuid(),
                    Name = poItem.ItemName,
                    SKU = stockSku,
                    CategoryId = category.Id,
                    QuantityAvailable = 0,
                    Unit = string.IsNullOrWhiteSpace(poItem.Unit) ? "pcs" : poItem.Unit,
                    ReorderLevel = poItem.ReorderLevel <= 0 ? 5 : poItem.ReorderLevel,
                    CreatedById = receivedBy,
                    CreatedAt = DateTime.UtcNow
                };

                _context.InventoryStocks.Add(stock);

                await _context.SaveChangesAsync();
            }

            stock.QuantityAvailable += itemDto.QuantityReceived;

            var goodsReceiptItem = new GoodsReceiptItem
            {
                Id = Guid.NewGuid(),
                PurchaseOrderItemId = poItem.Id,
                QuantityReceived = itemDto.QuantityReceived,
                Location = location,
                Condition = condition,
                Notes = itemDto.Notes,
                InventoryInserted = true,
                InventoryInsertedAt = DateTime.UtcNow
            };

            goodsReceipt.Items.Add(goodsReceiptItem);

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                StockId = stock.Id,
                QuantityChange = itemDto.QuantityReceived,
                Type = InventoryTransactionType.Receive,
                ReferenceId = purchaseOrder.Id,
                TransactionDate = DateTime.UtcNow,
                PerformedById = receivedBy,
                Remarks = $"Received {itemDto.QuantityReceived} {poItem.Unit ?? "pcs"} from PO {purchaseOrder.PurchaseOrderNumber}"
            });

            if (category.IsAssetTracked)
            {
                await CreateAssetTrackedInventoryItemsAsync(
                    poItem,
                    category,
                    stock,
                    itemDto,
                    location,
                    condition,
                    receivedBy);
            }

            receivedLookup[poItem.Id] = alreadyReceived + itemDto.QuantityReceived;
        }

        goodsReceipt.InventoryProcessingStatus = InventoryProcessingStatus.Complete;

        _context.GoodsReceipts.Add(goodsReceipt);

        UpdateDeliveryStatusFromLookup(
            purchaseOrder,
            receivedLookup,
            dto.NextExpectedDeliveryDate);

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return new GoodsReceiptResultDto
        {
            GoodsReceiptId = goodsReceipt.Id,
            PurchaseOrderId = purchaseOrder.Id,
            PurchaseOrderNumber = purchaseOrder.PurchaseOrderNumber,
            DeliveryStatus = purchaseOrder.DeliveryStatus.ToString(),
            InventoryProcessingStatus = goodsReceipt.InventoryProcessingStatus.ToString(),
            NextExpectedDeliveryDate = purchaseOrder.DeliveryDate,
            Message = purchaseOrder.DeliveryStatus == DeliveryStatus.PartiallyReceived
                ? "Partial goods receipt created successfully. Inventory updated. Remaining items scheduled for next delivery."
                : "Goods receipt created successfully. Inventory updated."
        };
    }

    public async Task<IEnumerable<PurchaseOrderDeliveryListDto>> GetReceivingDeliveriesAsync(
        PurchaseOrderDeliveryQueryDto query)
    {
        var targetDate = (query.Date ?? DateTime.UtcNow.Date).Date;

        var purchaseOrders = await _context.PurchaseOrders
            .Include(po => po.Vendor)
            .Include(po => po.Items)
                .ThenInclude(i => i.InventoryCategory)
            .Where(po =>
                po.DeliveryDate.HasValue &&
                po.DeliveryDate.Value.Date == targetDate &&
                po.DeliveryStatus != DeliveryStatus.Received)
            .OrderByDescending(po => po.DeliveryStatus == DeliveryStatus.PartiallyReceived)
            .ThenBy(po => po.DeliveryDate)
            .ToListAsync();

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

        var result = purchaseOrders
            .Select(po =>
            {
                var items = po.Items
                    .Select(item =>
                    {
                        var receivedQty = receivedLookup.TryGetValue(item.Id, out var qty)
                            ? qty
                            : 0;

                        var remainingQty = Math.Max(0, item.Quantity - receivedQty);

                        return new PurchaseOrderDeliveryItemDto
                        {
                            PurchaseOrderItemId = item.Id,
                            ItemName = item.ItemName,
                            SKU = null,
                            Quantity = item.Quantity,
                            InventoryCategoryId = item.InventoryCategoryId,
                            InventoryCategoryName = item.InventoryCategory?.Name,
                            IsAssetTracked = item.InventoryCategory?.IsAssetTracked ?? false,
                            OrderedQty = item.Quantity,
                            ReceivedQty = receivedQty,
                            RemainingQty = remainingQty,
                            UnitPrice = item.UnitPrice
                        };
                    })
                    .Where(x => x.RemainingQty > 0)
                    .ToList();

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
            })
            .Where(x => x.Items.Count > 0);

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
            result = result.Where(x =>
                x.Status.Equals(query.Status, StringComparison.OrdinalIgnoreCase));
        }

        return result.ToList();
    }

    public async Task<PurchaseOrderDeliveryListDto?> GetReceivingDeliveryByPurchaseOrderIdAsync(
        Guid purchaseOrderId)
    {
        var po = await _context.PurchaseOrders
            .Include(x => x.Vendor)
            .Include(x => x.Items)
                .ThenInclude(i => i.InventoryCategory)
            .FirstOrDefaultAsync(x => x.Id == purchaseOrderId);

        if (po == null)
            return null;

        var poItemIds = po.Items
            .Select(i => i.Id)
            .ToList();

        var receivedLookup = await _context.GoodsReceiptItems
            .Where(gri => poItemIds.Contains(gri.PurchaseOrderItemId))
            .GroupBy(gri => gri.PurchaseOrderItemId)
            .Select(g => new
            {
                PurchaseOrderItemId = g.Key,
                ReceivedQty = g.Sum(x => x.QuantityReceived)
            })
            .ToDictionaryAsync(x => x.PurchaseOrderItemId, x => x.ReceivedQty);

        var items = po.Items
            .Select(item =>
            {
                var receivedQty = receivedLookup.TryGetValue(item.Id, out var qty)
                    ? qty
                    : 0;

                var remainingQty = Math.Max(0, item.Quantity - receivedQty);

                return new PurchaseOrderDeliveryItemDto
                {
                    PurchaseOrderItemId = item.Id,
                    ItemName = item.ItemName,
                    SKU = null,
                    Quantity = item.Quantity,
                    InventoryCategoryId = item.InventoryCategoryId,
                    InventoryCategoryName = item.InventoryCategory?.Name,
                    IsAssetTracked = item.InventoryCategory?.IsAssetTracked ?? false,
                    OrderedQty = item.Quantity,
                    ReceivedQty = receivedQty,
                    RemainingQty = remainingQty,
                    UnitPrice = item.UnitPrice
                };
            })
            .Where(x => x.RemainingQty > 0)
            .ToList();

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

    public async Task<bool> ChangeArrivalDate(Guid purchaseOrderId, DateTime newArrivalDate)
    {
        var po = await _context.PurchaseOrders
            .FirstOrDefaultAsync(x => x.Id == purchaseOrderId);

        if (po == null)
            return false;

        if (po.DeliveryStatus == DeliveryStatus.Received)
            throw new Exception("Cannot change delivery date for a fully received purchase order.");

        if (newArrivalDate.Date < DateTime.UtcNow.Date)
            throw new Exception("Delivery date cannot be in the past.");

        po.DeliveryDate = newArrivalDate;

        await _context.SaveChangesAsync();

        return true;
    }

    private async Task CreateAssetTrackedInventoryItemsAsync(
        PurchaseOrderItem poItem,
        InventoryCategory category,
        InventoryStock stock,
        ReceivePurchaseOrderItemDto itemDto,
        string defaultLocation,
        string defaultCondition,
        Guid receivedBy)
    {
        var quantityReceived = itemDto.QuantityReceived;

        if (itemDto.AssetDetails == null || itemDto.AssetDetails.Count != quantityReceived)
        {
            throw new Exception(
                $"Please provide {quantityReceived} asset detail(s) for {poItem.ItemName}.");
        }

        var serialsInRequest = itemDto.AssetDetails
            .Where(x => !string.IsNullOrWhiteSpace(x.SerialNumber))
            .Select(x => x.SerialNumber!.Trim().ToLower())
            .ToList();

        if (serialsInRequest.Count != quantityReceived)
        {
            throw new Exception($"Serial number is required for all received {poItem.ItemName} items.");
        }

        if (serialsInRequest.Count != serialsInRequest.Distinct().Count())
        {
            throw new Exception($"Duplicate serial numbers found for {poItem.ItemName}.");
        }

        foreach (var assetDetail in itemDto.AssetDetails)
        {
            var serialNumber = assetDetail.SerialNumber!.Trim();

            var serialExists = await _context.InventoryItems
                .AnyAsync(x => x.SerialNumber != null &&
                               x.SerialNumber.ToLower() == serialNumber.ToLower());

            if (serialExists)
                throw new Exception($"Serial number {serialNumber} already exists.");

            var assetSku = await GenerateUniqueInventoryItemSkuAsync(poItem.ItemName, category.Id);

            var barcode = string.IsNullOrWhiteSpace(assetDetail.Barcode)
                ? assetSku
                : assetDetail.Barcode.Trim();

            var barcodeExists = await _context.InventoryItems
                .AnyAsync(x => x.Barcode != null &&
                               x.Barcode.ToLower() == barcode.ToLower());

            if (barcodeExists)
                throw new Exception($"Barcode {barcode} already exists.");

            var assetLocation = string.IsNullOrWhiteSpace(assetDetail.Location)
                ? defaultLocation
                : assetDetail.Location.Trim();

            var assetConditionText = string.IsNullOrWhiteSpace(assetDetail.Condition)
                ? defaultCondition
                : assetDetail.Condition.Trim();

            var inventoryCondition = ResolveInventoryCondition(assetConditionText);

            var asset = new InventoryItem
            {
                Id = Guid.NewGuid(),
                StockId = stock.Id,
                Name = poItem.ItemName,
                Description = assetDetail.Description?.Trim() ?? string.Empty,
                SKU = assetSku,
                Barcode = barcode,
                SerialNumber = serialNumber,
                Status = InventoryItemStatus.Available,
                Condition = inventoryCondition,
                Location = assetLocation,
                InventoryCategoryId = category.Id,
                CreatedById = receivedBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.InventoryItems.Add(asset);
        }
    }

    private static void UpdateDeliveryStatusFromLookup(
        PurchaseOrder purchaseOrder,
        Dictionary<Guid, int> receivedLookup,
        DateTime? nextExpectedDeliveryDate)
    {
        var totalOrdered = purchaseOrder.Items.Sum(x => x.Quantity);

        var totalReceived = purchaseOrder.Items.Sum(item =>
            receivedLookup.TryGetValue(item.Id, out var qty) ? qty : 0);

        if (totalReceived <= 0)
        {
            purchaseOrder.DeliveryStatus = DeliveryStatus.Pending;
        }
        else if (totalReceived < totalOrdered)
        {
            if (!nextExpectedDeliveryDate.HasValue)
                throw new Exception("Next expected delivery date is required for partial delivery.");

            if (nextExpectedDeliveryDate.Value.Date < DateTime.UtcNow.Date)
                throw new Exception("Next expected delivery date cannot be in the past.");

            purchaseOrder.DeliveryStatus = DeliveryStatus.PartiallyReceived;
            purchaseOrder.DeliveryDate = ToUtcRequired(nextExpectedDeliveryDate.Value);
        }
        else
        {
            purchaseOrder.DeliveryStatus = DeliveryStatus.Received;
            purchaseOrder.Status = PurchaseOrderStatus.Completed;
        }
    }

    private static InventoryItemCondition ResolveInventoryCondition(string condition)
    {
        return Enum.TryParse<InventoryItemCondition>(
            condition,
            true,
            out var parsedCondition)
            ? parsedCondition
            : InventoryItemCondition.Good;
    }

    private async Task<string> GenerateSkuAsync(string itemName, Guid categoryId)
    {
        var prefix = new string(
            itemName
                .Where(char.IsLetterOrDigit)
                .Take(3)
                .ToArray()
        ).ToUpper();

        if (string.IsNullOrWhiteSpace(prefix))
            prefix = "ITM";

        var count = await _context.InventoryStocks
            .CountAsync(x => x.CategoryId == categoryId);

        return $"{prefix}-{DateTime.UtcNow:yyyyMMdd}-{count + 1:D4}";
    }

    private async Task<string> GenerateUniqueStockSkuAsync(string itemName, Guid categoryId)
    {
        var baseSku = await GenerateSkuAsync(itemName, categoryId);

        var sku = baseSku;
        var counter = 1;

        while (
            await _context.InventoryStocks.AnyAsync(x => x.SKU == sku) ||
            _context.ChangeTracker.Entries<InventoryStock>()
                .Any(e => e.Entity.SKU == sku)
        )
        {
            sku = $"{baseSku}-STK-{counter:D3}";
            counter++;
        }

        return sku;
    }

    private async Task<string> GenerateUniqueInventoryItemSkuAsync(string itemName, Guid categoryId)
    {
        var baseSku = await GenerateSkuAsync(itemName, categoryId);

        var sku = baseSku;
        var counter = 1;

        while (
            await _context.InventoryItems.AnyAsync(x => x.SKU == sku) ||
            _context.ChangeTracker.Entries<InventoryItem>()
                .Any(e => e.Entity.SKU == sku)
        )
        {
            sku = $"{baseSku}-{counter:D3}";
            counter++;
        }

        return sku;
    }
    
    private static DateTime? ToUtc(DateTime? value)
    {
        if (!value.HasValue)
            return null;
    
        return ToUtcRequired(value.Value);
    }
    
    private static DateTime ToUtcRequired(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}