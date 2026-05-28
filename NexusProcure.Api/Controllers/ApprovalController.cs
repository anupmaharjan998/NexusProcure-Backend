using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.Enums;

namespace NexusProcure.Api.Controllers;

[Authorize]
public class ApprovalController : BaseApiController
{
    private readonly IRequisitionApprovalService _approvalService;

    public ApprovalController(IRequisitionApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    [HttpGet("{requisitionId}")]
        [Authorize(Policy = "APPROVE_REQUISITION")]
    public async Task<IActionResult> GetApprovalsForRequisition(Guid requisitionId)
    {
        var approvals = await _approvalService.GetApprovalsForRequisitionAsync(requisitionId);
        return Ok(approvals);
    }

    [HttpGet("pending/{userId}")]
    [Authorize(Policy = "APPROVE_REQUISITION")]
    public async Task<IActionResult> GetPendingApprovals(Guid userId)
    {
        var pending = await _approvalService.GetPendingApprovalsForRoleAsync(userId);
        return Ok(pending);
    }
}