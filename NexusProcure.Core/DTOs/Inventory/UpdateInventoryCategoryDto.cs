namespace NexusProcure.Core.DTOs.Inventory;

public class UpdateInventoryCategoryDto
{
    public string Name { get; set; }
    public string? CategoryCode { get; set; }
    public string? Description { get; set; }
    public int RiskWeight { get; set; }
    public bool IsAssetTracked { get; set; }
}