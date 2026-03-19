using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs.Procurement;

namespace NexusProcure.Api.Controllers;

public class PurchaseOrdersController : BaseApiController
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _purchaseOrderService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _purchaseOrderService.GetByIdAsync(id));

    // [HttpPost]
    // public async Task<IActionResult> Create(PurchaseOrderCreateDto dto) => Ok(await _purchaseOrderService.CreateAsync(dto));

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, string status) =>
        Ok(await _purchaseOrderService.UpdateStatusAsync(id, status));
}