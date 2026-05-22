namespace NexusProcure.Core.DTOs.Delegation;

public class CreateDelegationDto
{
    public Guid DelegateUserId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? Reason { get; set; }
}