namespace NexusProcure.Core.Entities.RequestForQuotations;

public class QuotationItem
{
    public Guid Id { get; set; }

    public Guid QuotationId { get; set; }

    public string ItemName { get; set; } = null!;
    public int Quantity { get; set; }

    public decimal TaxPercentage { get; set; }
    public decimal UnitPrice { get; set; }

    public int DeliveryDays { get; set; }

    public string? Remarks { get; set; }

    public decimal LineTotal => UnitPrice;

    /* Navigation */
    public Quotation Quotation { get; set; } = null!;
}
