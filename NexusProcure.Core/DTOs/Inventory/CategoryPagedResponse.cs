
namespace NexusProcure.Core.DTOs.Inventory;

public class CategoryPagedResponse
{
    public CategoryStats CategoryStats { get; set; }
    public List<InventoryCategoryDto> Categories { get; set; }
    
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    
}

public class CategoryStats
{
    public int TotalCategories { get; set; }
    public int TotalSubCategories { get; set; }
    public int TotalItems { get; set; }
}