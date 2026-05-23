using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.BackgroundJobs;

public class InventoryReceiptJob : IInventoryReceiptJob
{
    private readonly NexusProcureDbContext _context;
    private readonly IInventoryCodeService _inventoryCodeService;

    public InventoryReceiptJob(
        NexusProcureDbContext context,
        IInventoryCodeService inventoryCodeService)
    {
        _context = context;
        _inventoryCodeService = inventoryCodeService;
    }

    public async Task InsertReceivedItemsIntoInventoryAsync(Guid goodsReceiptId)
    {
        var goodsReceipt = await _context.GoodsReceipts
            .Include(gr => gr.Items)
                .ThenInclude(gri => gri.PurchaseOrderItem)
            .Include(gr => gr.PurchaseOrder)
            .FirstOrDefaultAsync(gr => gr.Id == goodsReceiptId);

        if (goodsReceipt == null)
            throw new Exception("Goods receipt not found.");

        await using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            goodsReceipt.InventoryProcessingStatus = InventoryProcessingStatus.Processing;
            goodsReceipt.InventoryProcessingError = null;
            await _context.SaveChangesAsync();

            foreach (var receiptItem in goodsReceipt.Items.Where(x => !x.InventoryInserted))
            {
                var poItem = receiptItem.PurchaseOrderItem;

                if (poItem == null)
                    throw new Exception("Purchase order item not found.");

                if (poItem.InventoryCategoryId == null)
                    throw new Exception($"Inventory category missing for {poItem.ItemName}.");

                var category = await _context.InventoryCategories
                    .FirstOrDefaultAsync(x => x.Id == poItem.InventoryCategoryId.Value);

                if (category == null)
                    throw new Exception($"Inventory category not found for {poItem.ItemName}.");

                var stock = await _context.InventoryStocks
                    .FirstOrDefaultAsync(x =>
                        x.Name == poItem.ItemName &&
                        x.CategoryId == category.Id);

                if (stock == null)
                {
                    var generatedStockCode = await _inventoryCodeService.GenerateSkuAndBarcodeAsync(
                        poItem.ItemName,
                        category.Id);

                    stock = new InventoryStock
                    {
                        Id = Guid.NewGuid(),
                        Name = poItem.ItemName,
                        SKU = generatedStockCode.Sku,
                        QuantityAvailable = 0,
                        CategoryId = category.Id,
                        Unit = string.IsNullOrWhiteSpace(poItem.Unit) ? "pcs" : poItem.Unit,
                        ReorderLevel = poItem.ReorderLevel <= 0 ? 5 : poItem.ReorderLevel,
                        CreatedById = goodsReceipt.ReceivedById,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.InventoryStocks.Add(stock);
                    await _context.SaveChangesAsync();
                }

                stock.QuantityAvailable += receiptItem.QuantityReceived;

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    StockId = stock.Id,
                    QuantityChange = receiptItem.QuantityReceived,
                    Type = InventoryTransactionType.Receive,
                    ReferenceId = goodsReceipt.Id,
                    TransactionDate = DateTime.UtcNow,
                    PerformedById = goodsReceipt.ReceivedById,
                    Remarks = $"Received from PO {goodsReceipt.PurchaseOrder.PurchaseOrderNumber}"
                });

                if (category.IsAssetTracked)
                {
                    for (int i = 0; i < receiptItem.QuantityReceived; i++)
                    {
                        var generated = await _inventoryCodeService.GenerateSkuAndBarcodeAsync(
                            poItem.ItemName,
                            category.Id);

                        var condition = InventoryItemCondition.Good;

                        if (!string.IsNullOrWhiteSpace(receiptItem.Condition))
                        {
                            Enum.TryParse(receiptItem.Condition, true, out condition);
                        }

                        var inventoryItem = new InventoryItem
                        {
                            Id = Guid.NewGuid(),
                            StockId = stock.Id,
                            Name = poItem.ItemName,
                            Description = string.Empty,
                            SerialNumber = null,
                            SKU = generated.Sku,
                            Barcode = generated.Barcode,
                            Status = InventoryItemStatus.Available,
                            Condition = condition,
                            Location = string.IsNullOrWhiteSpace(receiptItem.Location)
                                ? "Inventory"
                                : receiptItem.Location,
                            InventoryCategoryId = category.Id,
                            CreatedById = goodsReceipt.ReceivedById,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.InventoryItems.Add(inventoryItem);
                    }
                }

                receiptItem.InventoryInserted = true;
                receiptItem.InventoryInsertedAt = DateTime.UtcNow;
            }

            goodsReceipt.InventoryProcessingStatus = InventoryProcessingStatus.Complete;
            goodsReceipt.InventoryProcessingError = null;

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();

            goodsReceipt.InventoryProcessingStatus = InventoryProcessingStatus.Failed;
            goodsReceipt.InventoryProcessingError = ex.Message;

            await _context.SaveChangesAsync();
            throw;
        }
    }
}