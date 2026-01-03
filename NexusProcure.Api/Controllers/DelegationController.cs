using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Approval;

namespace NexusProcure.Api.Controllers;

public class DelegationController : Controller
{
    private readonly IDelegationService _delegationService;

    public DelegationController(IDelegationService delegationService)
    {
        _delegationService = delegationService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateDelegation([FromBody] CreateDelegationDto dto)
    {
        await _delegationService. CreateAsync(dto);
        return Ok(new { message = "Delegation created" });
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetDelegations(Guid userId)
    {
        var delegations = await _delegationService.GetActiveDelegationsAsync();
        return Ok(delegations);
    }

    // [HttpDelete("{id}")]
    // public async Task<IActionResult> DeleteDelegation(Guid id)
    // {
    //     await _delegationService.DeleteAsync(id);
    //     return NoContent();
    // }
}
