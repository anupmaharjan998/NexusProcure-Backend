namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryStockQueryParams
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }

    // InStock / LowStock / OutOfStock
    public string? Status { get; set; }

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}