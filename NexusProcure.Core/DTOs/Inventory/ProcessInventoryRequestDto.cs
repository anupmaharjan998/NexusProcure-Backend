namespace NexusProcure.Core.DTOs.Inventory;

public class ProcessInventoryRequestDto
{
    public List<ProcessInventoryRequestItemDto> Items { get; set; } = new();
}