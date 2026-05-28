namespace NexusProcure.Core.DTOs.Inventory;

public class MyAssignedInventoryItemDetailDto
{
    public Guid Id { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public string? CategoryName { get; set; }

    public string? SKU { get; set; }

    public string? Barcode { get; set; }

    public string? SerialNumber { get; set; }

    public string? Department { get; set; }

    public string? AssignedTo { get; set; }

    public string? Location { get; set; }

    public string? Condition { get; set; }

    public string? Status { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}