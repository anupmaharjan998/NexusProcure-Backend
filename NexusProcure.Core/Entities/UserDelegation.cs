namespace NexusProcure.Core.Entities;

public class UserDelegation : BaseEntity
{
    public Guid Id { get; set; }

    // Who is delegating (Manager)
    public Guid UserId { get; set; }
    public User User { get; set; }

    // Who will act on behalf
    public Guid DelegateUserId { get; set; }
    public User DelegateUser { get; set; }

    // Time-bound delegation
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public string? Reason { get; set; }
}