using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.DTOs.PurchaseOrder;

namespace NexusProcure.Api.Controllers;

public class PurchaseOrderReceiptController : BaseApiController
{
    private readonly IPurchaseOrderReceiptService _purchaseOrderReceiptService;

    public PurchaseOrderReceiptController(IPurchaseOrderReceiptService purchaseOrderReceiptService)
    {
        _purchaseOrderReceiptService = purchaseOrderReceiptService;
    }

    [HttpPost("{purchaseOrderId:guid}/receive")]
    public async Task<IActionResult> ReceivePurchaseOrder(
        Guid purchaseOrderId,
        [FromBody] ReceivePurchaseOrderDto dto)
    {
        if (purchaseOrderId != dto.PurchaseOrderId)
            return BadRequest("Purchase order mismatch.");

        var userIdClaim = User.FindFirstValue("userId");

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized("User id missing.");

        var result = await _purchaseOrderReceiptService.ReceivePurchaseOrderAsync(dto, userId);

        return Ok(result);
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayDeliveries(
        [FromQuery] string? search,
        [FromQuery] string? status)
    {
        var result = await _purchaseOrderReceiptService.GetReceivingDeliveriesAsync(
            new PurchaseOrderDeliveryQueryDto
            {
                Search = search,
                Status = status,
                Date = DateTime.UtcNow.Date
            });

        return Ok(result);
    }

    [HttpGet("{purchaseOrderId:guid}")]
    public async Task<IActionResult> GetDeliveryByPurchaseOrderId(Guid purchaseOrderId)
    {
        var result = await _purchaseOrderReceiptService
            .GetReceivingDeliveryByPurchaseOrderIdAsync(purchaseOrderId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPut("{purchaseOrderId:guid}/delivery-date")]
    public async Task<IActionResult> UpdateDeliveryDate(
        Guid purchaseOrderId,
        [FromBody] UpdatePurchaseOrderArrivalDateDto dto)
    {
        var updated = await _purchaseOrderReceiptService.ChangeArrivalDate(
            purchaseOrderId,
            dto.NewArrivalDate);

        if (!updated)
            return NotFound("Purchase Order not found.");

        return Ok(new
        {
            message = "Delivery date updated successfully."
        });
    }
}