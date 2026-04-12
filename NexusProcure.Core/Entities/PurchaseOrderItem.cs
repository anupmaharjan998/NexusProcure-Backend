using NexusProcure.Core.Entities.Inventory;

namespace NexusProcure.Core.Entities;

public class PurchaseOrderItem
{
    public Guid Id { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; }

    public string ItemName { get; set; } = string.Empty;
    public decimal TaxPercentage { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    
    public Guid? InventoryCategoryId { get; set; }
    public InventoryCategory? InventoryCategory { get; set; }
}
