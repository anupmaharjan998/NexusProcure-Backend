namespace NexusProcure.Core.Entities;

public class ApprovalLevel
{
    public Guid Id { get; set; }
    public string LevelName { get; set; } = string.Empty;

    public Guid RoleId { get; set; }
    public Role Role { get; set; }
}
