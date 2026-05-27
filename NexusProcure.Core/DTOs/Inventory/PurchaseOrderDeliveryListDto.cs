namespace NexusProcure.Core.DTOs.Inventory;


public class PurchaseOrderDeliveryListDto
{
    public Guid Id { get; set; }

    public string PurchaseOrderNumber { get; set; } = string.Empty;

    public string VendorName { get; set; } = string.Empty;

    public DateTime? ExpectedDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public int TotalItems { get; set; }

    public List<PurchaseOrderDeliveryItemDto> Items { get; set; } = new();
}
