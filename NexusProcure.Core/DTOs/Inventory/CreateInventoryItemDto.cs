namespace NexusProcure.Core.DTOs.Inventory;

public class CreateInventoryItemDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    
}