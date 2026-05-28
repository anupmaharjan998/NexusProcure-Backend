using System;
using System.ComponentModel.DataAnnotations;

namespace NexusProcure.Core.Entities.Inventory;

public class ProcurementRequestItem : BaseEntity
{
    public Guid Id { get; set; }

    public Guid ProcurementRequestId { get; set; }
    public ProcurementRequest ProcurementRequest { get; set; } = null!;

    public Guid InventoryRequestItemId { get; set; }
    public InventoryRequestItem InventoryRequestItem { get; set; } = null!;

    public Guid InventoryStockId { get; set; }
    public InventoryStock InventoryStock { get; set; } = null!;

    public int RequestedQuantity { get; set; }

    public int AvailableQuantity { get; set; }

    public int RequiredProcurementQuantity { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}