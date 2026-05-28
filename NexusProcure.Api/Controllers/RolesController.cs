using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;

namespace NexusProcure.Api.Controllers;

public class RolesController : BaseApiController
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll() => Ok(await _roleService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var role = await _roleService.GetByIdAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    [HttpPost]
    [Authorize(Policy = "CREATE_ROLE")]
    public async Task<IActionResult> Create(CreateRoleDto dto)
    {
        var role = await _roleService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "EDIT_ROLE")]
    public async Task<IActionResult> Update(Guid id, UpdateRoleDto dto)
    {
        var updated = await _roleService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "DELETE_ROLE")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _roleService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
    
    [HttpGet("check-role-name")]
    public async Task<IActionResult> CheckRoleName(
        [FromQuery] string name,
        [FromQuery] Guid? excludeRoleId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(new { message = "Role name is required" });
        }
    
        var normalizedRoleName = name.Trim().ToLower();
    
        var exists = await _roleService.CheckRoleNameAsync(normalizedRoleName, excludeRoleId);
    
        return Ok(new { exists });
    }
}