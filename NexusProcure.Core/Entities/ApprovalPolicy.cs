using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities;

public class ApprovalPolicy
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    
    public RiskLevel RiskLevel { get; set; }

    public Guid ApprovalLevelId { get; set; }
    public ApprovalLevel ApprovalLevel { get; set; }

    public int SequenceOrder { get; set; }
    public int EscalationHours { get; set; } // SLA

    public bool IsActive { get; set; }
}

