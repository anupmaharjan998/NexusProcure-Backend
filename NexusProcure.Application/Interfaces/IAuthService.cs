using NexusProcure.Application.DTOs;

namespace NexusProcure.Application.Interfaces;

public interface IAuthService
{
    Task<UserResponse?> LoginAsync(LoginRequest request);
}