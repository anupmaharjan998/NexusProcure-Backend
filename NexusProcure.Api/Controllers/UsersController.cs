using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;

namespace NexusProcure.Api.Controllers;

public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _userService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    [Authorize(Policy = "CREATE_USER")]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        try
        {
            var newUser = await _userService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "EDIT_USER")]
    public async Task<IActionResult> Update(Guid id, UpdateUserDto dto)
    {
        var updated = await _userService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "DELETE_USER")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
    
    [Authorize]
    [HttpPost("upload-profile-picture")]
    public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile fileRequest)
    {
        if (fileRequest == null || fileRequest.Length == 0)
            return BadRequest(new { message = "File is required" });

        //var email = User.FindFirst("email")?.Value;
        var email = User.FindFirstValue(ClaimTypes.Email);

        var url = await _userService.UploadProfilePictureAsync(email, fileRequest);

        return Ok(new { imageUrl = url });
    }
    
    
    
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UserProfileUpdate(UserUpdateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        var updated = await _userService.UserProfileUpdateAsync(userId, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    
}