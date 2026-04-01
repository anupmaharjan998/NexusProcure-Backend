namespace NexusProcure.Core.Entities.Inventory;

public class GoodsReceipt
{
    public Guid Id { get; set; }

    public Guid PurchaseOrderId { get; set; }

    public DateTime ReceivedDate { get; set; }

    public ICollection<GoodsReceiptItem> Items { get; set; }
}