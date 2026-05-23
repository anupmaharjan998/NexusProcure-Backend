namespace NexusProcure.Core.DTOs.Inventory;

public class AdjustInventoryStockDto
{
    // Positive = add stock, negative = remove stock
    public int QuantityChange { get; set; }

    public string? Remarks { get; set; }
}