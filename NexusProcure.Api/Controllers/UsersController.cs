using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.DTOs;
using NexusProcure.Application.Interfaces;

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
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        var newUser = await _userService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateUserDto dto)
    {
        var updated = await _userService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _userService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}