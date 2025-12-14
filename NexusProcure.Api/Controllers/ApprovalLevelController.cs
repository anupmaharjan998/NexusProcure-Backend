using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;

namespace NexusProcure.Api.Controllers;

public class ApprovalLevelController : BaseApiController
{
    private readonly IApprovalLevelService _approvalLevelService;

    public ApprovalLevelController(IApprovalLevelService approvalLevelService)
    {
        _approvalLevelService = approvalLevelService;
    }

    [HttpPost("create-approval-level")]
    public async Task<IActionResult> Create([FromBody] ApprovalLeveRequestlDto dto)
    {
        var approvalLevel = await _approvalLevelService.CreateAsync(dto);
        return Ok(approvalLevel);
    }

    [HttpGet("get-all-level")]
    public async Task<IActionResult> GetAll()
    {
        var levels = await _approvalLevelService.GetAllAsync();
        return Ok(levels);
    }
}