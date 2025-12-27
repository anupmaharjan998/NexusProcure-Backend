namespace NexusProcure.Core.Entities;

public class ApprovalPolicy
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

    public Guid ApprovalLevelId { get; set; }
    public ApprovalLevel ApprovalLevel { get; set; }

    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }

    public int SequenceOrder { get; set; } // 1, 2, 3...
    public bool IsActive { get; set; } = true;
}