using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Approval;

namespace NexusProcure.Api.Controllers;

public class ApprovalPolicyController : BaseApiController
{
    private readonly IApprovalPolicyService _policyService;

    public ApprovalPolicyController(IApprovalPolicyService policyService)
    {
        _policyService = policyService;
    }

    [HttpGet]
    [Authorize(Policy = "VIEW_POLICIES")]
    public async Task<IActionResult> GetPolicies()
    {
        var policies = await _policyService.GetPoliciesAsync();
        return Ok(policies);
    }

    [HttpPost]
    [Authorize(Policy = "ADD_POLICIES")]
    public async Task<IActionResult> Create([FromBody] ApprovalPolicyCreateDto dto)
    {
        await _policyService.CreateAsync(dto);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "DELETE_POLICIES")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _policyService.DeleteAsync(id);
        return NoContent();
    }
}