using NexusProcure.Core.DTOs.Vendor;

namespace NexusProcure.Core.DTOs.RFQ;

public class PublicRfqDto
{
    public string RfqNumber { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime SubmissionDeadline { get; set; }
    public RfqVendorDto Vendor { get; set; }
    public List<PublicRfqItemDto> Items { get; set; } = [];
}