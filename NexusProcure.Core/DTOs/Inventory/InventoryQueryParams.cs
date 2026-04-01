namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryQueryParams
{
    public string? Search { get; set; }
    public Guid? CategoryId { get; set; }
    public string? Status { get; set; } // InStock, LowStock, OutOfStock
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}