namespace NexusProcure.Core.Entities.RequestForQuotations;

public class RfqVendor
{
    public Guid Id { get; set; }

    public Guid RfqId { get; set; }

    public Guid VendorId { get; set; }

    /* Secure Access */
    public string AccessToken { get; set; } = null!;
    public DateTime TokenExpiresAt { get; set; }

    /* Vendor Progress */
    public string Status { get; set; } = "Invited";
    // Invited, Viewed, Submitted, Withdrawn

    public DateTime? ViewedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }

    /* Navigation */
    public RequestForQuotation Rfq { get; set; } = null!;
    public Vendor Vendor { get; set; } = null!;
}
