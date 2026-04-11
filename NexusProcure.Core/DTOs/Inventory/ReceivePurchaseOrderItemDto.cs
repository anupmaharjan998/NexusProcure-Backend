namespace NexusProcure.Core.DTOs.Inventory;

public class ReceivePurchaseOrderItemDto
{
    public Guid PurchaseOrderItemId { get; set; }
    public int QuantityReceived { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Condition { get; set; } = "Good";
    public string? Notes { get; set; }
}