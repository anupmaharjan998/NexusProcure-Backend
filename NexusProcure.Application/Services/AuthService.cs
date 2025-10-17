using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NexusProcure.Application.Interfaces;
using NexusProcure.Infrastructure.Data;
using NexusProcure.Shared.Helper;
using Microsoft.AspNetCore.Identity;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Services;

public class AuthService : IAuthService
{
    private readonly NexusProcureDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(NexusProcureDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<UserResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .Include(r => r.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (user == null || passwordVerificationResult == PasswordVerificationResult.Failed)
            return null;

        var token = JwtTokenGenerator.GenerateToken(user, _config["Jwt:Key"]!);

        return new UserResponse
        {
            User = new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name
            },
            Token = token
        };
    }
}