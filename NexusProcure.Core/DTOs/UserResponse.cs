namespace NexusProcure.Core.DTOs;

public class UserResponse
{
    public UserResponseDto User { get; set; }
    public string Token { get; set; } = string.Empty;
}


public class UserResponseDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}