using NexusProcure.Core.DTOs;

namespace NexusProcure.Application.Services.Procurement;

public class ApprovalDto
{
    public Guid Id { get; set; }
    public Guid RequisitionId { get; set; }

    public Guid? ApprovedById { get; set; }
    public UserResponseDto ApprovedBy { get; set; }
    
    public string ApprovedByName { get; set; } = string.Empty;
    public DateTime? ActionedAt { get; set; }
    
    public Guid RoleId { get; set; }
    public RoleDto Role { get; set; }

    public DateTime ApprovedDate { get; set; }
    public string Status { get; set; } 
    public string Comments { get; set; }
}


public class ApprovalRequestDto
{
    public Guid ApproverId { get; set; }
    public string Decision { get; set; } = "Approved";
    public string Comments { get; set; } = string.Empty;
}