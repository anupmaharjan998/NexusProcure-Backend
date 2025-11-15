namespace NexusProcure.Core.DTOs;

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AssignPermissionDto
{
    public Guid RoleId { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
}