namespace NexusProcure.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? RoleName { get; set; }
    public string? DepartmentName { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUserDto
{
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public Guid? DepartmentId { get; set; }
}

public class UpdateUserDto
{
    public string? Email { get; set; }
    public Guid? RoleId { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool? IsActive { get; set; }
}