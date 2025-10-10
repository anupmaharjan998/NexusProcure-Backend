using NexusProcure.Core.Entities;

namespace NexusProcure.Application.DTOs;

public class UserResponse
{
    public User User { get; set; }
    public string Token { get; set; } = string.Empty;
}