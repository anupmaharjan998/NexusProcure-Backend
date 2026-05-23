using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.Inventory;

public class CreateInventoryRequestDto
{
    public string Purpose { get; set; }
    public RequestPriority Priority { get; set; }
    public Guid? StockId { get; set; }          // For consumables
    public Guid? InventoryItemId { get; set; }  // For assets
    public int Quantity { get; set; }
    public List<CreateInventoryRequestItemDto> Items { get; set; }
}