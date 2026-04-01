namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryItemDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string SKU { get; set; }
    public string Barcode { get; set; }
    public string SerialNumber { get; set; }

    public string Category { get; set; }

    public string Status { get; set; }
    public string Condition { get; set; }
    public string Location { get; set; }

    public string AssignedTo { get; set; }
    public DateTime? AssignedDate { get; set; }

    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
}