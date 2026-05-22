using System;

namespace NexusProcure.Core.Entities.Inventory;

public enum StockTransactionType
{
    In,
    Out,
    Adjustment
}

public class StockTransaction
{
    public Guid Id { get; set; }

    public Guid InventoryItemId { get; set; }

    public StockTransactionType Type { get; set; }

    public int Quantity { get; set; }

    public DateTime Date { get; set; }

    public string Reference { get; set; }
}