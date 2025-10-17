namespace NexusProcure.Core.DTOs;

public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<string>? Permissions { get; set; } // For role → permission mapping
}

public class CreateRoleDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateRoleDto
{
    public string Name { get; set; } = string.Empty;
}