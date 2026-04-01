namespace NexusProcure.Core.DTOs.Inventory;

public class AssignItemDto
{
    public Guid InventoryItemId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
}