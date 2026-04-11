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

        try
        {
            goodsReceipt.InventoryProcessingStatus = InventoryProcessingStatus.Processing;
            goodsReceipt.InventoryProcessingError = null;
            await _context.SaveChangesAsync();

            foreach (var receiptItem in goodsReceipt.Items.Where(x => !x.InventoryInserted))
            {
                var poItem = receiptItem.PurchaseOrderItem;
                if (poItem == null)
                    throw new Exception("Purchase order item not found for goods receipt item.");

                // TODO:
                // Replace this category resolution logic with a proper mapping in your system.
                // Example: derive from requisition category, PO metadata, or explicit inventory category on PO item.
                var inventoryCategory = await _context.InventoryCategories.FirstOrDefaultAsync();
                if (inventoryCategory == null)
                    throw new Exception("No inventory category available for inventory insertion.");

                for (int i = 0; i < receiptItem.QuantityReceived; i++)
                {
                    var generated = await _inventoryCodeService.GenerateSkuAndBarcodeAsync(
                        poItem.ItemName,
                        inventoryCategory.Id);

                    var inventoryItem = new InventoryItem
                    {
                        Id = Guid.NewGuid(),
                        Name = poItem.ItemName,
                        Description = string.Empty,
                        SerialNumber = null,
                        SKU = generated.Sku,
                        Barcode = generated.Barcode,
                        Status = "Available",
                        AssignedToId = null,
                        AssignedDate = DateTime.MinValue,
                        Location = string.IsNullOrWhiteSpace(receiptItem.Location) ? "Inventory" : receiptItem.Location,
                        Condition = string.IsNullOrWhiteSpace(receiptItem.Condition) ? "Good" : receiptItem.Condition,
                        InventoryCategoryId = inventoryCategory.Id,
                        CreatedById = goodsReceipt.ReceivedById
                    };

                    _context.InventoryItems.Add(inventoryItem);
                    await _context.SaveChangesAsync();

                    // For quantity > 1 this stores the last inserted item ID.
                    // If you want one-to-many linkage from receipt item to all inserted inventory items,
                    // introduce a separate linking table later.
                    receiptItem.InventoryItemId = inventoryItem.Id;
                }

                receiptItem.InventoryInserted = true;
                receiptItem.InventoryInsertedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            goodsReceipt.InventoryProcessingStatus = InventoryProcessingStatus.Complete;
            goodsReceipt.InventoryProcessingError = null;
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            goodsReceipt.InventoryProcessingStatus = InventoryProcessingStatus.Failed;
            goodsReceipt.InventoryProcessingError = ex.Message;
            await _context.SaveChangesAsync();
            throw;
        }
    }
}