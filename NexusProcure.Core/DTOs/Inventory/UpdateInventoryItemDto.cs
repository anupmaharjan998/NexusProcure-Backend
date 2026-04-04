namespace NexusProcure.Core.DTOs.Inventory;

public class UpdateInventoryItemDto
{
    public string Name { get; set; }
    public Guid InventoryCategoryId { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public string Condition { get; set; }
    public string Location { get; set; }
    public string? SerialNumber { get; set; }
}