using System;

namespace NexusProcure.Core.Entities.Inventory;

public class InventoryRequestIssuedItem : BaseEntity
{
    public Guid Id { get; set; }

    public Guid InventoryRequestItemId { get; set; }
    public InventoryRequestItem InventoryRequestItem { get; set; } = null!;

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;
}