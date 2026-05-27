using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.ProcurementRequest;
using NexusProcure.Core.DTOs.ProcurementRequest;

namespace NexusProcure.Api.Controllers;

[ApiController]
[Route("api/procurement-requests")]
[Authorize]
public class ProcurementRequestController : ControllerBase
{
    private readonly IProcurementRequestService _procurementRequestService;

    public ProcurementRequestController(
        IProcurementRequestService procurementRequestService)
    {
        _procurementRequestService = procurementRequestService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _procurementRequestService.GetAllAsync();

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _procurementRequestService.GetByIdAsync(id);

        if (result == null)
            return NotFound(new
            {
                message = "Procurement request not found."
            });

        return Ok(result);
    }

    [HttpPost("{id:guid}/create-requisition")]
    [Authorize(Roles = "Admin,ProcurementOfficer,Procurement Officer")]
    public async Task<IActionResult> CreateRequisition(
        Guid id,
        [FromBody] CreateRequisitionFromProcurementRequestDto dto)
    {
        if (dto == null)
        {
            return BadRequest(new
            {
                message = "Create requisition payload is required."
            });
        }

        if (dto.Items == null || !dto.Items.Any())
        {
            return BadRequest(new
            {
                message = "At least one item with estimated cost is required."
            });
        }

        var invalidCostItem = dto.Items.FirstOrDefault(x => x.EstimatedUnitCost <= 0);

        if (invalidCostItem != null)
        {
            return BadRequest(new
            {
                message = "Estimated unit cost must be greater than zero for every item."
            });
        }

        var userId = GetCurrentUserId();

        var requisitionId = await _procurementRequestService.CreateRequisitionAsync(
            id,
            userId,
            dto
        );

        return Ok(new
        {
            message = "Requisition created successfully.",
            requisitionId
        });
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectProcurementRequestDto dto)
    {
        if (dto == null)
            return BadRequest(new
            {
                message = "Reject payload is required."
            });

        if (string.IsNullOrWhiteSpace(dto.Reason))
            return BadRequest(new
            {
                message = "Reject reason is required."
            });

        var userId = GetCurrentUserId();

        await _procurementRequestService.RejectAsync(
            id,
            userId,
            dto.Reason
        );

        return Ok(new
        {
            message = "Procurement request rejected successfully."
        });
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue("userId");
        

        if (string.IsNullOrWhiteSpace(userIdClaim))
            throw new UnauthorizedAccessException("User id not found in token.");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user id in token.");

        return userId;
    }
}