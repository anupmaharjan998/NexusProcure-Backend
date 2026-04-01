namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryItemDto
{
    public Guid Id { get; set; }

    public string SKU { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }

    public string SerialNumber { get; set; }
    public string Barcode { get; set; }

    public string Status { get; set; }

    public string AssignedTo { get; set; }

    public string Location { get; set; }
}