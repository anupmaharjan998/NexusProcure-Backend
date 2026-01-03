using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.Approval;

public class ApprovalPolicyCreateDto
{
    public Guid CategoryId { get; set; }
    public Guid ApprovalLevelId { get; set; }
    public RiskLevel RiskLevel { get; set; }   // ✅ ADD
    public int SequenceOrder { get; set; }
}


public class ApprovalPolicyResponseDto
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; }
    public string RoleName { get; set; }
    public RiskLevel RiskLevel { get; set; }
    public int SequenceOrder { get; set; }
}