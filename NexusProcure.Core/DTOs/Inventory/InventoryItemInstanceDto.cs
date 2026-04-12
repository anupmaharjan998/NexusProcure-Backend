namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryItemInstanceDto
{
    public Guid Id { get; set; }
    public string SerialNumber { get; set; }
    public string Barcode { get; set; }
    public string Status { get; set; }
    public string Location { get; set; }
}