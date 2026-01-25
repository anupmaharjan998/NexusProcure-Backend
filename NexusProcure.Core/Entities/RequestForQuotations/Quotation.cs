namespace NexusProcure.Core.Entities.RequestForQuotations;

public class Quotation
{
    public Guid Id { get; set; }
    public Guid RfqId { get; set; }

    public Guid RfqVendorId { get; set; }

    public DateTime SubmittedAt { get; set; }

    public string SubmissionMethod { get; set; } = "Excel";
    // Excel, WebForm

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "NPR";

    /* Navigation */
    public RfqVendor RfqVendor { get; set; } = null!;

    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
}
