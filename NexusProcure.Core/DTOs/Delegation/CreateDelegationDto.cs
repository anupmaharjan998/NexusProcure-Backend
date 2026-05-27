namespace NexusProcure.Core.DTOs.Delegation;

public class CreateDelegationDto
{
    public Guid? UserId { get; set; }

    public Guid DelegateUserId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Scope { get; set; } = "All";

    public string? Reason { get; set; }
}