namespace NexusProcure.Core.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string? Description { get; set; }
    public int RiskWeight { get; set; } // 0–30
    public ICollection<Vendor>? Vendors { get; set; }
}