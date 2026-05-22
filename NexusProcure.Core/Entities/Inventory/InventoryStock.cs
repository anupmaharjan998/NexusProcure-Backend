using System;
using System.Collections.Generic;

namespace NexusProcure.Core.Entities.Inventory;

public class InventoryStock : BaseEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;

    public int QuantityAvailable { get; set; }

    public Guid CategoryId { get; set; }
    public InventoryCategory Category { get; set; }

    public string Unit { get; set; } = "pcs"; // pcs, box, etc.

    public int ReorderLevel { get; set; }

    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    public ICollection<InventoryItem> Items { get; set; } = new List<InventoryItem>();
    public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
}