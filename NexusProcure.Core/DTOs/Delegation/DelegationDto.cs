namespace NexusProcure.Core.DTOs.Delegation;

public class DelegationDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public string UserName { get; set; }

    public Guid DelegateUserId { get; set; }
    public string DelegateUserName { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}