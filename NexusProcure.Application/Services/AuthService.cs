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

        if (user == null)
            return null;

        var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return null;

        var token = JwtTokenGenerator.GenerateToken(user, _config["Jwt:Key"]!);
        
        var permissions = await  _context.RolePermissions.Where(x=>x.RoleId == user.RoleId)
            .Include(p=>p.Permission)
            .Select(l=> l.Permission.Key).ToListAsync();
        return new UserResponse
        {
            User = new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name
            },
            Token = token,
            Permissions = permissions
        };
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return false;

        
        if (string.IsNullOrWhiteSpace(request.NewPassword) ||
            request.NewPassword != request.ConfirmNewPassword)
        {
            return false;
        }
        
        var hasher = new PasswordHasher<User>();
        var verify = hasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        if (verify == PasswordVerificationResult.Failed)
            return false;

        
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, request.NewPassword) == PasswordVerificationResult.Success)
            return false;

        
        user.PasswordHash = hasher.HashPassword(user, request.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }
}