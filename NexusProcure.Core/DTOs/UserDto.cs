using System.ComponentModel.DataAnnotations;

namespace NexusProcure.Core.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public Guid? RoleId { get; set; }
    public string? RoleName { get; set; }

    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }

    public bool IsActive { get; set; }

    public string? ProfileImageUrl { get; set; }

    // Manager Info
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
}

public class CreateUserDto
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [RegularExpression(@"^[0-9]{10}$")]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    [Required]
    public Guid RoleId { get; set; }

    public Guid? DepartmentId { get; set; }
    
    public Guid? ManagerId { get; set; }
}

public class UpdateUserDto
{
    public string? FullName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    public Guid? RoleId { get; set; }
    public Guid? DepartmentId { get; set; }

    public Guid? ManagerId { get; set; } 

    [RegularExpression(@"^[0-9]{10}$")]
    public string? PhoneNumber { get; set; }

    public bool IsActive { get; set; }
}

public class ForgotPasswordRequestDto
{
    [Required]
    public string Email { get; set; } = string.Empty;
}

public class ResetPasswordRequestDto
{
    [Required]
    public Guid Token { get; set; }
    [Required]
    public string NewPassword { get; set; } = string.Empty;
}


public class UserUpdateDto
{
    [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
}

