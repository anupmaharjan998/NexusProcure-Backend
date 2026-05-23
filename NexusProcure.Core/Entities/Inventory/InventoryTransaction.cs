using System;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities.Inventory;

public class InventoryTransaction : BaseEntity
{
    public Guid Id { get; set; }

    public Guid StockId { get; set; }
    public InventoryStock Stock { get; set; }

    public int QuantityChange { get; set; } // + or -

    public InventoryTransactionType Type { get; set; } // ISSUE / RECEIVE / ADJUSTMENT

    public Guid? ReferenceId { get; set; } // RequestId / ProcurementId

    public string? Remarks { get; set; }

    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

    public Guid? PerformedById { get; set; }
    public User? PerformedBy { get; set; }
}