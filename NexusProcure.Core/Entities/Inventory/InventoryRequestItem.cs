using System;
using System.Collections.Generic;

namespace NexusProcure.Core.Entities.Inventory;
 
public class InventoryRequestItem : BaseEntity
{
    public Guid Id { get; set; }

    public Guid InventoryRequestId { get; set; }
    public InventoryRequest InventoryRequest { get; set; } = null!;

    // 🔹 For consumables
    public Guid StockId { get; set; }
    public InventoryStock Stock { get; set; }

    public int QuantityRequested { get; set; }
    public int QuantityIssued { get; set; }

    public ICollection<InventoryRequestIssuedItem> IssuedItems { get; set; } = new List<InventoryRequestIssuedItem>();
}