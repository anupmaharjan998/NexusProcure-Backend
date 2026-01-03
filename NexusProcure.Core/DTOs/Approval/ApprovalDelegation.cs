namespace NexusProcure.Core.DTOs.Approval;

public class ApprovalDelegation
{
    public Guid Id { get; set; }

    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? CategoryId { get; set; }
    public Guid? ApprovalLevelId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}


public class CreateDelegationDto
{
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public Guid? CategoryId { get; set; }
    public Guid? ApprovalLevelId { get; set; }
}


public class DelegationResponseDto
{
    public Guid Id { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}
