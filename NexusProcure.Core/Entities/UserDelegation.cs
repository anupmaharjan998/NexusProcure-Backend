namespace NexusProcure.Core.Entities;

public class UserDelegation : BaseEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid DelegateUserId { get; set; }
    public User DelegateUser { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Scope { get; set; } = "All";

    public string? Reason { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
}