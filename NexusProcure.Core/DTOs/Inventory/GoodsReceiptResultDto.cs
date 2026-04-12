namespace NexusProcure.Core.DTOs.Inventory;

public class GoodsReceiptResultDto
{
    public Guid GoodsReceiptId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;
    public string DeliveryStatus { get; set; } = string.Empty;
    public string InventoryProcessingStatus { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}