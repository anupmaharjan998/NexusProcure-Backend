namespace NexusProcure.Core.DTOs.Inventory;

public class CreateInventoryStockDto
{
    public string Name { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public int OpeningQuantity { get; set; }

    public string Unit { get; set; } = "pcs";

    public int ReorderLevel { get; set; } = 5;
}