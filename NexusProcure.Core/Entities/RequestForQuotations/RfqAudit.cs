namespace NexusProcure.Core.Entities.RequestForQuotations;

public class RfqAudit
{
    public Guid Id { get; set; }

    public Guid RfqId { get; set; }

    public string Action { get; set; } = null!;
    // Created, VendorInvited, QuoteSubmitted, Awarded

    public DateTime CreatedAt { get; set; }

    public string PerformedBy { get; set; } = "System";

    public RequestForQuotation Rfq { get; set; } = null!;
}
