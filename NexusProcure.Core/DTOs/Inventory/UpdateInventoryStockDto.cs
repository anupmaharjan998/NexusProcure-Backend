namespace NexusProcure.Core.DTOs.Inventory;

public class UpdateInventoryStockDto
{
    public string Name { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public string Unit { get; set; } = "pcs";

    public int ReorderLevel { get; set; } = 5;
}