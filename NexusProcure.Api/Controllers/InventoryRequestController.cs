using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.API.Controllers.Inventory;

[ApiController]
[Route("api/inventory-requests")]
[Authorize]
public class InventoryRequestController : ControllerBase
{
    private readonly IInventoryRequestService _service;

    public InventoryRequestController(IInventoryRequestService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInventoryRequestDto dto)
    {
        var userId = GetUserId();

        var requestId = await _service.CreateAsync(userId, dto);

        return Ok(new
        {
            RequestId = requestId,
            Message = "Inventory request created successfully."
        });
    }

    [HttpGet("{requestId:guid}")]
    public async Task<IActionResult> GetById(Guid requestId)
    {
        var result = await _service.GetByIdAsync(requestId);

        if (result == null)
            return NotFound("Inventory request not found.");

        return Ok(result);
    }

    [HttpPost("{requestId:guid}/manager-approve")]
    public async Task<IActionResult> ApproveByManager(Guid requestId)
    {
        var userId = GetUserId();

        await _service.ApproveByManagerAsync(requestId, userId);

        return Ok(new
        {
            Message = "Inventory request approved by manager."
        });
    }

    [HttpPost("{requestId:guid}/manager-reject")]
    public async Task<IActionResult> RejectByManager(
        Guid requestId,
        [FromBody] RejectInventoryRequestDto dto)
    {
        var userId = GetUserId();

        await _service.RejectByManagerAsync(requestId, userId, dto.Remarks);

        return Ok(new
        {
            Message = "Inventory request rejected by manager."
        });
    }

    [HttpPost("{requestId:guid}/process")]
    public async Task<IActionResult> ProcessByInventoryManager(
        Guid requestId,
        [FromBody] ProcessInventoryRequestDto dto)
    {
        var userId = GetUserId();

        await _service.ProcessByInventoryManagerAsync(requestId, userId, dto);

        return Ok(new
        {
            Message = "Inventory request processed successfully."
        });
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue("userId");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User id missing.");

        return userId;
    }
}