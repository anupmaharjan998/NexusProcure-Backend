namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryStockPagedResponse
{
    public List<InventoryStockDto> Items { get; set; } = new();

    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public InventoryStockStatsDto Stats { get; set; } = new();
}

public class InventoryStockStatsDto
{
    public int TotalStocks { get; set; }
    public int LowStock { get; set; }
    public int OutOfStock { get; set; }
    public int AssetTrackedStocks { get; set; }
}