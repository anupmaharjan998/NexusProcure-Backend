using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.Inventory;

namespace NexusProcure.API.Controllers.Inventory;

[ApiController]
[Route("api/inventory-stocks")]
[Authorize]
public class InventoryStockController : ControllerBase
{
    private readonly IInventoryStockService _service;

    public InventoryStockController(IInventoryStockService service)
    {
        _service = service;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableStock()
    {
        var result = await _service.GetAvailableStockAsync();
        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var result = await _service.GetLowStockAsync();
        return Ok(result);
    }
}