namespace NexusProcure.Core.DTOs.Inventory;

public class PurchaseOrderDeliveryItemDto
{
    public Guid PurchaseOrderItemId { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public string? SKU { get; set; }

    public decimal Quantity { get; set; }

    public Guid? InventoryCategoryId { get; set; }

    public string? InventoryCategoryName { get; set; }

    public bool IsAssetTracked { get; set; }

    public decimal OrderedQty { get; set; }

    public decimal ReceivedQty { get; set; }

    public decimal RemainingQty { get; set; }

    public decimal UnitPrice { get; set; }
}