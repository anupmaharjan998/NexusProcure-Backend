namespace NexusProcure.Core.DTOs.Inventory;

public class CreateCategoryDto
{
    public string Name { get; set; }
    public int RiskWeight { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
}