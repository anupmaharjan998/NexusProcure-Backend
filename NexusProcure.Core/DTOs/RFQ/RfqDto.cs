using NexusProcure.Core.Entities.RequestForQuotations;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.RFQ;

public class RfqDto
{
    public Guid Id { get; set; }
    public string RfqNumber { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime SubmissionDeadline { get; set; }
    public int TotalQuotationsRecieved { get; set; }
    public RfqStatus Status { get; set; } = RfqStatus.Open; 

}