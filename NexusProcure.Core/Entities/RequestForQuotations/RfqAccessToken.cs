namespace NexusProcure.Core.Entities.RequestForQuotations;

public class RfqAccessToken
{
    public Guid Id { get; set; }

    public Guid RfqId { get; set; }
    public Guid VendorId { get; set; }

    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }
    public bool IsExpired { get; set; }
    public DateTime CreatedAt { get; set; }

    public RequestForQuotation Rfq { get; set; }
    public Vendor Vendor { get; set; }
}
