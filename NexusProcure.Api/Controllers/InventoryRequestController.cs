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

    [HttpGet("my")]
    public async Task<IActionResult> GetMyRequests()
    {
        var userId = GetUserId();

        var result = await _service.GetMyRequestsAsync(userId);

        return Ok(result);
    }

    [HttpGet("manager-pending")]
    public async Task<IActionResult> GetPendingForManager()
    {
        var managerId = GetUserId();

        var result = await _service.GetPendingForManagerAsync(managerId);

        return Ok(result);
    }

    [HttpGet("inventory-pending")]
    public async Task<IActionResult> GetApprovedForInventoryManager()
    {
        var result = await _service.GetApprovedForInventoryManagerAsync();

        return Ok(result);
    }

    [HttpGet("{requestId:guid}")]
    public async Task<IActionResult> GetById(Guid requestId)
    {
        var result = await _service.GetByIdAsync(requestId);

        if (result == null)
            return NotFound("Inventory request not found.");

        return Ok(result);
    }

    [HttpGet("stocks/{stockId:guid}/available-assets")]
    public async Task<IActionResult> GetAvailableAssets(Guid stockId)
    {
        var result = await _service.GetAvailableAssetsByStockAsync(stockId);

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
    
    [HttpGet("manager-shortage-pending")]
    public async Task<IActionResult> GetShortagePendingForManager()
    {
        var managerId = GetUserId();

        var result = await _service.GetShortagePendingForManagerAsync(managerId);

        return Ok(result);
    }

    [HttpPost("{requestId:guid}/shortage/send-procurement")]
    public async Task<IActionResult> SendShortageToProcurement(
        Guid requestId,
        [FromBody] ResolveInventoryShortageDto dto)
    {
        var managerId = GetUserId();

        await _service.SendShortageToProcurementAsync(requestId, managerId, dto.Remarks);

        return Ok(new
        {
            Message = "Inventory request sent to procurement."
        });
    }

    [HttpPost("{requestId:guid}/shortage/reject")]
    public async Task<IActionResult> RejectShortage(
        Guid requestId,
        [FromBody] ResolveInventoryShortageDto dto)
    {
        var managerId = GetUserId();

        await _service.RejectShortageAsync(requestId, managerId, dto.Remarks);

        return Ok(new
        {
            Message = "Inventory request rejected due to insufficient quantity."
        });
    }
    
    [HttpGet("my-assigned")]
    public async Task<ActionResult<List<MyAssignedInventoryItemDto>>> GetMyAssignedItems()
    {
        var userId = GetUserId();

        var items = await _service.GetMyAssignedItemsAsync(userId);

        return Ok(items);
    }
    
    [HttpGet("my-assigned/{itemId:guid}")]
    public async Task<ActionResult<MyAssignedInventoryItemDetailDto>> GetMyAssignedItemDetail(
        Guid itemId)
    {
        var userId = GetUserId();
    
        var item = await _service.GetMyAssignedItemDetailAsync(userId, itemId);
    
        if (item == null)
        {
            return NotFound(new
            {
                Message = "Assigned inventory item not found."
            });
        }
    
        return Ok(item);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue("userId");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User id missing.");

        return userId;
    }
}