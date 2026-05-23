using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Delegation;

namespace NexusProcure.Api.Controllers;

public class DelegationController : Controller
{
    private readonly IDelegationService _delegationService;

    public DelegationController(IDelegationService delegationService)
    {
        _delegationService = delegationService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateDelegation(Guid userId, [FromBody] CreateDelegationDto dto)
    {
        await _delegationService.CreateAsync(userId, dto);
        return Ok(new { message = "Delegation created" });
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetDelegations(Guid userId)
    {
        var delegations = await _delegationService.GetActiveDelegateAsync(userId);
        return Ok(delegations);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> DeactivateDelegation(Guid id)
    {
        await _delegationService.DeactivateAsync(id);
        return NoContent();
    }
}
