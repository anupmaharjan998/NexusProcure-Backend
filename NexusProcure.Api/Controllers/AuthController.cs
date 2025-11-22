using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;

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
        if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
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

        var success = await _authService.ChangePasswordAsync(email, request);
        if (!success)
        {
            // For security, avoid revealing whether email exists or password mismatch
            return Unauthorized(new { message = "Invalid credentials or unable to change password" });
        }

        return Ok(new { message = "Password changed successfully" });
    }
    
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto dto)
    {
        await _authService.RequestPasswordResetAsync(dto);
        return Ok(new { Message = "If this email exists, a password reset token has been sent." });
    }

    [HttpGet("verify-token")]
    public async Task<IActionResult> VerifyToken(string token)
    {
        var valid = await _authService.RequestVerifyTokenAsync(token);
        if (!valid)
        {
            return Unauthorized(new { message = "Invalid token." });
        }

        return Ok(new { message = "Valid token." });
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto dto)
    {
        await _authService.ResetPasswordAsync(dto);
        return Ok(new { Message = "Password has been reset successfully." });
    }

}
