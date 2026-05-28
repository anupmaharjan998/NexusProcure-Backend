namespace NexusProcure.Core.DTOs.Inventory;

public class MyAssignedInventoryItemDto
{
    public Guid Id { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public string? CategoryName { get; set; }

    public string? SerialNumber { get; set; }
    public string? Department { get; set; }

    public string? Barcode { get; set; }

    public string? Location { get; set; }

    public string? Condition { get; set; }

    public DateTime? AssignedAt { get; set; }
}