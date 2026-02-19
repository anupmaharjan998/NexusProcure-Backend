namespace NexusProcure.Core.Entities.RequestForQuotations;

public class Quotation
{
    public Guid Id { get; set; }
    public Guid RfqId { get; set; }
    public RequestForQuotation RequestForQuotation { get; set; }

    public Guid RfqVendorId { get; set; }

    public DateTime SubmittedAt { get; set; }

    public string SubmissionMethod { get; set; } = "Excel";
    // Excel, WebForm

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "NPR";
    
    public string Notes { get; set; } = null!;
    public DateTime DeliveryDate { get; set; }
    
    public string SignedBy { get; set; } = null!;
    public string IpAddress { get; set; } = null!;

    public bool IsSelected { get; set; }

    /* Navigation */
    public RfqVendor RfqVendor { get; set; } = null!;

    public ICollection<QuotationItem> Items { get; set; } = new List<QuotationItem>();
}
