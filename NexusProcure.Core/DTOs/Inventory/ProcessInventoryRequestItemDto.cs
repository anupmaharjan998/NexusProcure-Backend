namespace NexusProcure.Core.DTOs.Inventory;

public class ProcessInventoryRequestItemDto
{
    public Guid InventoryRequestItemId { get; set; }

    public List<Guid> InventoryItemIds { get; set; } = new();
}