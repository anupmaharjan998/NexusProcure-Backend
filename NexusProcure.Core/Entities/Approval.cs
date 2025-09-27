namespace NexusProcure.Core.Entities;

public class Approval
{
    public Guid Id { get; set; }
    public Guid RequisitionId { get; set; }
    public Requisition Requisition { get; set; }

    public Guid ApprovedById { get; set; }
    public User ApprovedBy { get; set; }

    public DateTime ApprovedDate { get; set; }
    public string Decision { get; set; } = string.Empty; // Approved, Rejected
    public string Comments { get; set; } = string.Empty;
}
