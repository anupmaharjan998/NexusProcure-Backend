namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryRequestItemDto
{
    public Guid Id { get; set; }

    public Guid StockId { get; set; }

    public string StockName { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int QuantityRequested { get; set; }

    public int QuantityIssued { get; set; }

    public int QuantityAvailable { get; set; }

    public bool IsAssetTracked { get; set; }

    public List<IssuedInventoryItemDto> IssuedItems { get; set; } = new();
}

public class IssuedInventoryItemDto
{
    public Guid InventoryItemId { get; set; }

    public string SKU { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;
}