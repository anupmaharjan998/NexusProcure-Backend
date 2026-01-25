namespace NexusProcure.Core.DTOs.RFQ;

public class RfqEmailDto
{
    public string VendorEmail { get; set; } = default!;
    public string VendorName { get; set; } = default!;

    public string RfqNumber { get; set; } = default!;
    public string RfqLink { get; set; } = default!;
    public DateTime SubmissionDeadline { get; set; }
}
