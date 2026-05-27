namespace NexusProcure.Core.DTOs.Inventory;

public class AvailableInventoryItemDto
{
    public Guid Id { get; set; }

    public string SKU { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}