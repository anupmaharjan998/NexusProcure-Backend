using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;

namespace NexusProcure.Api.Controllers;

public class PermissionsController : BaseApiController
{
    private readonly IPermissionService _permissionService;

    public PermissionsController(IPermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _permissionService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var permission = await _permissionService.GetByIdAsync(id);
        return permission == null ? NotFound() : Ok(permission);
    }

    // 🎯 Assign permissions to a role
    [HttpPost("assign/{roleId}")]
    public async Task<IActionResult> AssignToRole(Guid roleId, [FromBody] List<Guid> permissionIds)
    {
        var success = await _permissionService.AssignPermissionsToRole(roleId, permissionIds);
        return success ? Ok("Permissions updated") : NotFound("Role not found");
    }
}