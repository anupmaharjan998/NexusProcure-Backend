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

    [HttpGet("geyById/{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var role = await _approvalLevelService.GetByIdAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ApprovalLeveRequestlDto dto)
    {
        var updated = await _approvalLevelService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _approvalLevelService.DeleteAsync(id);
        return result ? NoContent() : NotFound();
    }
}