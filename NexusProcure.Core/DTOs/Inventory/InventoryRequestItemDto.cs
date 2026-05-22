namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryRequestItemDto
{
    public Guid Id { get; set; }
    public Guid StockId { get; set; }
    public string StockName { get; set; } = string.Empty;
    public int QuantityRequested { get; set; }
    public int QuantityIssued { get; set; }
    public bool IsAssetTracked { get; set; }
}