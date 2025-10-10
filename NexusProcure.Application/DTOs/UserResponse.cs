using NexusProcure.Core.Entities;

namespace NexusProcure.Application.DTOs;

public class UserResponse
{
    public UserDto User { get; set; }
    public string Token { get; set; } = string.Empty;
}


public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}