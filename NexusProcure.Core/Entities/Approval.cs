using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities;

public class Approval
{
    public Guid Id { get; set; }

    // public Guid RequisitionId { get; set; }
    // public Requisition Requisition { get; set; }    

    
    public Guid ReferenceId { get; set; }
    public ApprovalReferenceType ReferenceType { get; set; }

    
    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    // public Guid? AssignedToUserId { get; set; }
    // public User AssignedToUser { get; set; }

    public Guid? ApprovedById { get; set; }
    public User ApprovedBy { get; set; }

    public DateTime AssignedAt { get; set; }
    public DateTime? ActionedAt { get; set; }
    public int SequenceOrder { get; set; }

    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    public string? Comments { get; set; }
    
    public bool IsActive { get; set; }
    public bool Escalated { get; set; } = false;
    public DateTime? EscalatedAt { get; set; }
}
