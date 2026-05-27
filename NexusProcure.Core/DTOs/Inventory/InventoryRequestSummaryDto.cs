namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryRequestSummaryDto
{
    public Guid Id { get; set; }

    public string RequestedBy { get; set; } = string.Empty;

    public string Department { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int TotalItems { get; set; }

    public DateTime CreatedAt { get; set; }
}