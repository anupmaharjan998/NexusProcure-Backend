using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities.RequestForQuotations;

public class RequestForQuotation
{
    public Guid Id { get; set; }

    public Guid RequisitionId { get; set; }

    public string RfqNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime SubmissionDeadline { get; set; }

    public RfqStatus Status { get; set; } = RfqStatus.Open; 
    // Open, Closed, Awarded

    /* Navigation */
    public Requisition Requisition { get; set; } = null!;

    public ICollection<RfqVendor> Vendors { get; set; } = new List<RfqVendor>();

    public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
}
