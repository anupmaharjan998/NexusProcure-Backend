namespace NexusProcure.Core.DTOs.Approval;

public class ApprovalPolicyCreateDto
{
    public Guid CategoryId { get; set; }
    public Guid ApprovalLevelId { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public int SequenceOrder { get; set; }
}


public class ApprovalPolicyResponseDto
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; }
    public string RoleName { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public int SequenceOrder { get; set; }
}