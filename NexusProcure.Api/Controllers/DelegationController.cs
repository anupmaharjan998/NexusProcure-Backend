using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Delegation;

namespace NexusProcure.Api.Controllers;

[ApiController]
[Route("api/delegations")]
[Authorize]
public class DelegationController : ControllerBase
{
    private readonly IDelegationService _delegationService;

    public DelegationController(IDelegationService delegationService)
    {
        _delegationService = delegationService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateDelegation([FromBody] CreateDelegationDto dto)
    {
        var currentUserClaim = GetCurrentUserClaim();

        var result = await _delegationService.CreateAsync(currentUserClaim, dto);

        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetDelegations()
    {
        var currentUserClaim = GetCurrentUserClaim();

        var delegations = await _delegationService.GetVisibleDelegationsAsync(currentUserClaim);

        return Ok(delegations);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyDelegations()
    {
        var currentUserClaim = GetCurrentUserClaim();

        var delegations = await _delegationService.GetMyDelegationsAsync(currentUserClaim);

        return Ok(delegations);
    }

    [HttpGet("active-delegate/{userId:guid}")]
    public async Task<IActionResult> GetActiveDelegate(Guid userId)
    {
        var delegateUser = await _delegationService.GetActiveDelegateAsync(userId);

        return Ok(delegateUser);
    }

    [HttpPut("{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateDelegation(Guid id)
    {
        var currentUserClaim = GetCurrentUserClaim();

        var success = await _delegationService.DeactivateAsync(currentUserClaim, id);

        if (!success)
        {
            return NotFound(new
            {
                message = "Delegation not found or you are not allowed to revoke it."
            });
        }

        return NoContent();
    }

    [HttpGet("permissions")]
    public async Task<IActionResult> GetDelegationPermissions()
    {
        var currentUserClaim = GetCurrentUserClaim();

        var permissions = await _delegationService.GetPermissionsAsync(currentUserClaim);

        return Ok(permissions);
    }

    private string GetCurrentUserClaim()
    {
        var claimValue = User.FindFirstValue("userId");

        if (string.IsNullOrWhiteSpace(claimValue))
        {
            throw new UnauthorizedAccessException("User claim was not found.");
        }

        return claimValue;
    }
}