namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string CategoryCode { get; set; }
    public int RiskWeight { get; set; }
    public int TotalItems { get; set; }
    public bool IsAssetTracked { get; set; }

    public List<InventoryCategoryDto> SubCategories { get; set; }
}