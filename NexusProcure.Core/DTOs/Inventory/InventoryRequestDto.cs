namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryRequestDto
{
    public Guid Id { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public List<InventoryRequestItemDto> Items { get; set; } = new();
}