namespace NexusProcure.Core.Entities.Inventory;

public class Stock
{
    public Guid Id { get; set; }

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; }

    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
}