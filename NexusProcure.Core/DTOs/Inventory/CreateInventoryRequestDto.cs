using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.Inventory;

public class CreateInventoryRequestDto
{
    public string Purpose { get; set; } = string.Empty;

    public RequestPriority Priority { get; set; }

    public List<CreateInventoryRequestItemDto> Items { get; set; } = new();
}

public class CreateInventoryRequestItemDto
{
    public Guid StockId { get; set; }

    public int Quantity { get; set; }
}