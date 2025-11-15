using System.ComponentModel.DataAnnotations;

namespace NexusProcure.Core.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public bool IsActive { get; set; }
}

public class CreateUserDto
{
    [Required(ErrorMessage = "Please enter full name.")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter username.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter email address.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public bool IsActive { get; set; }

    [Required(ErrorMessage = "Please select the role.")]
    public Guid RoleId { get; set; }

    public Guid? DepartmentId { get; set; }
}

public class UpdateUserDto
{
    [EmailAddress]
    public string? Email { get; set; }
    public Guid? RoleId { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool IsActive { get; set; }
}