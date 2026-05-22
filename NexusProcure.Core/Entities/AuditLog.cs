namespace NexusProcure.Core.Entities;

public class AuditLog : BaseEntity
{
    public Guid Id { get; set; }

    public string EntityName { get; set; } // e.g. "UserDelegation"
    public Guid EntityId { get; set; }

    public string Action { get; set; } // Created, Updated, Deleted, Expired

    public Guid? PerformedBy { get; set; } // User who triggered action

    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
