namespace NexusProcure.Core.Entities.Inventory;

public class GoodsReceiptItem
{
    public Guid Id { get; set; }

    public Guid GoodsReceiptId { get; set; }
    public GoodsReceipt GoodsReceipt { get; set; } = null!;

    public Guid PurchaseOrderItemId { get; set; }
    public PurchaseOrderItem PurchaseOrderItem { get; set; } = null!;

    public Guid? InventoryItemId { get; set; }
    public InventoryItem? InventoryItem { get; set; }

    public int QuantityReceived { get; set; }

    public string Location { get; set; } = string.Empty;
    public string Condition { get; set; } = "Good";
    public string? Notes { get; set; }

    public bool InventoryInserted { get; set; }
    public DateTime? InventoryInsertedAt { get; set; }
}