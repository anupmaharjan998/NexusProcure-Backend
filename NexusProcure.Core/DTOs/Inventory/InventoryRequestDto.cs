namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryRequestDto
{
    public Guid Id { get; set; }

    public string RequestedBy { get; set; } = string.Empty;

    public Guid RequestedById { get; set; }

    public string Department { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<InventoryRequestItemDto> Items { get; set; } = new();
}