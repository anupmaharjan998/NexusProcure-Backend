using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Application.Services.Procurement;

namespace NexusProcure.Api.Controllers;

public class RequisitionApprovalController : BaseApiController
{
    private readonly IRequisitionApprovalService _approvalService;

    public RequisitionApprovalController(IRequisitionApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    [HttpGet("{id}/approvals")]
    public async Task<IActionResult> GetApprovals(Guid id)
    {
        var approvals = await _approvalService.GetApprovalsForRequisitionAsync(id);
        return Ok(approvals);
    }

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveRequisition(Guid id, [FromBody] ApprovalRequestDto dto)
    {
        try
        {
            await _approvalService.ApproveRequisitionAsync(id, dto.ApproverId, dto.RoleId, dto.Decision, dto.Comments);
            return Ok(new { message = "Approval recorded successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingApprovalsForRole()
    {
        var userId = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        var pending = await _approvalService.GetPendingApprovalsForRoleAsync(userId);
        return Ok(pending);
    }
}