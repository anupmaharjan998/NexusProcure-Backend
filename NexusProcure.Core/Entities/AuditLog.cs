namespace NexusProcure.Core.Entities;

public class AuditLog
{
    public Guid Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public Guid PerformedById { get; set; }
    public User PerformedBy { get; set; }

    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}
