namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryPagedResponse
{
    public List<InventoryItemDto> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public InventoryStatsDto Stats { get; set; }
}