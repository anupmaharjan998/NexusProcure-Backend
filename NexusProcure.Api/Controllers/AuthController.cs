using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace NexusProcure.Api.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (result == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(result);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.CurrentPassword) ||
            string.IsNullOrWhiteSpace(request.NewPassword) ||
            string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
        {
            return BadRequest(new { message = "All fields are required" });
        }

        
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrWhiteSpace(email))
        {
            return Unauthorized(new { message = "Email not found in token" });
        }

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return BadRequest(new { message = "New password and confirmation do not match" });
        }

        
        request.Email = email;

        var success = await _authService.ChangePasswordAsync(request);
        if (!success)
        {
            // For security, avoid revealing whether email exists or password mismatch
            return Unauthorized(new { message = "Invalid credentials or unable to change password" });
        }

        return Ok(new { message = "Password changed successfully" });
    }
}
