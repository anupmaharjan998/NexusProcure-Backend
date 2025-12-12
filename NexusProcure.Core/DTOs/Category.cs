namespace NexusProcure.Core.DTOs;

public class CategoryRequest
{
    public string Name { get; set; }
    public string Type { get; set; } // Vendor, Inventory, Both
    public string? Description { get; set; }
}

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string? Description { get; set; }
}
