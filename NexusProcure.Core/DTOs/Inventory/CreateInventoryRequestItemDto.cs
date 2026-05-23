namespace NexusProcure.Core.DTOs.Inventory;

public class CreateInventoryRequestItemDto
{
    public Guid StockId { get; set; }
    public int Quantity { get; set; }
}