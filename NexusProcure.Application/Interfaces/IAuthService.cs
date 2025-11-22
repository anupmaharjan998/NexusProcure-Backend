using NexusProcure.Core.DTOs;

namespace NexusProcure.Application.Interfaces;

public interface IAuthService
{
    Task<UserResponse?> LoginAsync(LoginRequest request);
    Task<bool> ChangePasswordAsync(string email, ChangePasswordRequest request);
}