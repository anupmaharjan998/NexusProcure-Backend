using NexusProcure.Core.Entities.Inventory;

namespace NexusProcure.Core.Entities.RequestForQuotations;

public class QuotationItem
{
    public Guid Id { get; set; }

    public Guid QuotationId { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal TaxPercentage { get; set; }

    public decimal UnitPrice { get; set; }

    public int DeliveryDays { get; set; }

    public string? Remarks { get; set; }

    public Guid? InventoryCategoryId { get; set; }
    public InventoryCategory? InventoryCategory { get; set; }
    
    public decimal LineTotal =>
        (UnitPrice * Quantity) + (UnitPrice * Quantity * TaxPercentage / 100m);

    public Quotation Quotation { get; set; } = null!;
}