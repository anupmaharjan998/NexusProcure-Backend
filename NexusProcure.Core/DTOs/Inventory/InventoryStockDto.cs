using System;

namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryStockDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public int QuantityAvailable { get; set; }
    public string Unit { get; set; } = "pcs";
    public int ReorderLevel { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public bool IsAssetTracked { get; set; }
    public string Status { get; set; } = string.Empty;

}