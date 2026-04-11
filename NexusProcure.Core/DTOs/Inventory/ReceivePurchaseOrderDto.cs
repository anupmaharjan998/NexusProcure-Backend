namespace NexusProcure.Core.DTOs.Inventory;

public class ReceivePurchaseOrderDto
{
    public Guid PurchaseOrderId { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public string? Notes { get; set; }
    public List<ReceivePurchaseOrderItemDto> Items { get; set; } = new();
}