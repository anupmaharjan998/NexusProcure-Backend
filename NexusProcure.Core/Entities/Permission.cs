namespace NexusProcure.Core.Entities;

public class Permission
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty; // e.g. "VIEW_USERS"
    public string Description { get; set; } = string.Empty;

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}