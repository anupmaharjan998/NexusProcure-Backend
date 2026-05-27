using NexusProcure.Core.Entities.Inventory;

namespace NexusProcure.Core.Entities;

public class RequisitionItem
{
    public Guid Id { get; set; }

    public Guid RequisitionId { get; set; }
    public Requisition Requisition { get; set; } = null!;

    public Guid InventoryStockId { get; set; }
    public InventoryStock InventoryStock { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal EstimatedCost { get; set; }

    public string? Remarks { get; set; }
}