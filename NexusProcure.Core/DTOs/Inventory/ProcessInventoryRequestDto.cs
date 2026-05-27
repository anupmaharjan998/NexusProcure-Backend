namespace NexusProcure.Core.DTOs.Inventory;

public class ProcessInventoryRequestDto
{
    public List<ProcessInventoryRequestItemDto> Items { get; set; } = new();
}

public class ProcessInventoryRequestItemDto
{
    public Guid InventoryRequestItemId { get; set; }

    public List<Guid> InventoryItemIds { get; set; } = new();
}