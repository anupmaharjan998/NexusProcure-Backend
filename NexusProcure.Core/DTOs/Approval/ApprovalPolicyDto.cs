using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.Approval;

public class ApprovalPolicyCreateDto
{
    public Guid CategoryId { get; set; }
    public Guid RoleId { get; set; }
    public string RiskLevel { get; set; }   // ✅ ADD
    public int EscalationHours { get; set; }
    public int SequenceOrder { get; set; }
    public bool IsActive { get; set; }
}


public class ApprovalPolicyResponseDto
{
    public Guid Id { get; set; }
    public string CategoryName { get; set; }
    public string RoleName { get; set; }
    public string RiskLevel { get; set; }
    public int SequenceOrder { get; set; }
    public int EscalationHours { get; set; }
    public bool IsActive { get; set; }
}