namespace NexusProcure.Core.DTOs.RFQ;

public class RfqResponseDto
{
    public Guid Id { get; set; }
    public string RfqNumber { get; set; } = default!;
    public Guid RequisitionId { get; set; }

    public DateTime SubmissionDeadline { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<RfqVendorDto> Vendors { get; set; } = [];
}
