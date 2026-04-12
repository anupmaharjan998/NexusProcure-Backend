namespace NexusProcure.Core.DTOs.Inventory;

public class PurchaseOrderDeliveryItemDto
{
    public Guid PurchaseOrderItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? SKU { get; set; }
    public int OrderedQty { get; set; }
    public int ReceivedQty { get; set; }
    public int RemainingQty { get; set; }
    public decimal UnitPrice { get; set; }
}