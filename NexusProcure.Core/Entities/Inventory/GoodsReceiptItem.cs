namespace NexusProcure.Core.Entities.Inventory;

public class GoodsReceiptItem
{
    public Guid Id { get; set; }

    public Guid GoodsReceiptId { get; set; }
    public GoodsReceipt GoodsReceipt { get; set; }

    public Guid InventoryItemId { get; set; }

    public int QuantityReceived { get; set; }
}