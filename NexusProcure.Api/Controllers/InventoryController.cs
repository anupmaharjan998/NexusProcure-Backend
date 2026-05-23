using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Api.Controllers;

[Authorize]
public class InventoryController : BaseApiController
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    // ============================================================
    // STOCK / CATALOG
    // ============================================================

    [HttpGet("stocks")]
    public async Task<IActionResult> GetStocks([FromQuery] InventoryStockQueryParams query)
    {
        var result = await _inventoryService.GetStocksAsync(query);
        return Ok(result);
    }

    [HttpGet("stocks/available")]
    public async Task<IActionResult> GetAvailableStocks()
    {
        var result = await _inventoryService.GetAvailableStocksAsync();
        return Ok(result);
    }

    [HttpPost("stocks")]
    [Authorize(Policy = "ADD_INVENTORY_ITEM")]
    public async Task<IActionResult> CreateStock([FromBody] CreateInventoryStockDto dto)
    {
        var userId = GetUserId();
        var result = await _inventoryService.CreateStockAsync(dto, userId);
        return Ok(result);
    }

    [HttpPut("stocks/{id:guid}")]
    [Authorize(Policy = "UPDATE_INVENTORY_ITEM")]
    public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateInventoryStockDto dto)
    {
        var userId = GetUserId();
        var result = await _inventoryService.UpdateStockAsync(id, dto, userId);
        return Ok(result);
    }

    [HttpPost("stocks/{id:guid}/adjust")]
    [Authorize(Policy = "UPDATE_INVENTORY_ITEM")]
    public async Task<IActionResult> AdjustStock(Guid id, [FromBody] AdjustInventoryStockDto dto)
    {
        var userId = GetUserId();
        await _inventoryService.AdjustStockAsync(id, dto, userId);

        return Ok(new
        {
            Message = "Stock adjusted successfully."
        });
    }

    // ============================================================
    // ASSET ITEMS
    // ============================================================

    [HttpGet("assets")]
    public async Task<IActionResult> GetAssets([FromQuery] InventoryQueryParams query)
    {
        var result = await _inventoryService.GetInventoryAsync(query);
        return Ok(result);
    }

    [HttpGet("assets/{id:guid}")]
    public async Task<IActionResult> GetAssetById(Guid id)
    {
        var result = await _inventoryService.GetInventoryItemById(id);
        return Ok(result);
    }

    [HttpPost("assets")]
    [Authorize(Policy = "ADD_INVENTORY_ITEM")]
    public async Task<IActionResult> CreateAsset([FromBody] CreateInventoryItemDto dto)
    {
        var userId = GetUserId();
        var result = await _inventoryService.CreateItemAsync(dto, userId);
        return Ok(result);
    }

    [HttpPut("assets/{id:guid}")]
    [Authorize(Policy = "UPDATE_INVENTORY_ITEM")]
    public async Task<IActionResult> UpdateAsset(Guid id, [FromBody] UpdateInventoryItemDto dto)
    {
        var userId = GetUserId();
        var result = await _inventoryService.UpdateItemAsync(id, dto, userId);
        return Ok(result);
    }

    [HttpGet("assets/by-stock/{stockId:guid}")]
    public async Task<IActionResult> GetAvailableAssetsByStock(Guid stockId)
    {
        var result = await _inventoryService.GetAvailableAssetsByStockAsync(stockId);
        return Ok(result);
    }

    [HttpPost("assets/{id:guid}/assign")]
    [Authorize(Policy = "ASSIGN_ASSET")]
    public async Task<IActionResult> AssignAsset(Guid id, [FromBody] AssignInventoryItemDto dto)
    {
        var userId = GetUserId();

        var result = await _inventoryService.AssignItemAsync(id, dto, userId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost("assets/{id:guid}/unassign")]
    [Authorize(Policy = "UNASSIGN_ASSET")]
    public async Task<IActionResult> UnassignAsset(Guid id)
    {
        var userId = GetUserId();

        var result = await _inventoryService.UnassignItemAsync(id, userId);

        if (result == null)
            return NotFound();

        return Ok(result);
    }

    // ============================================================
    // CATEGORIES
    // ============================================================

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] CategoryQueryParams query)
    {
        var result = await _inventoryService.GetCategoriesAsync(query);
        return Ok(result);
    }

    [HttpPost("categories")]
    [Authorize(Policy = "ADD_CATEGORIES")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var userId = GetUserId();
        await _inventoryService.CreateCategoryAsync(dto, userId);

        return Ok(new
        {
            Message = "Category created successfully."
        });
    }

    [HttpPut("categories/{id:guid}")]
    [Authorize(Policy = "UPDATE_CATEGORIES")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateInventoryCategoryDto dto)
    {
        var userId = GetUserId();
        await _inventoryService.UpdateCategoryAsync(id, dto, userId);

        return Ok(new
        {
            Message = "Category updated successfully."
        });
    }

    [HttpDelete("categories/{id:guid}")]
    [Authorize(Policy = "DELETE_CATEGORIES")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _inventoryService.DeleteCategoryAsync(id);

        return Ok(new
        {
            Message = "Category deleted successfully."
        });
    }

    [HttpGet("categories/leaf")]
    public async Task<IActionResult> GetLeafCategories()
    {
        var result = await _inventoryService.GetLeafCategories();
        return Ok(result);
    }

    [HttpGet("categories/leaf-dropdown")]
    public async Task<IActionResult> GetLeafCategoriesForDropdown()
    {
        var result = await _inventoryService.GetLeafCategoriesAsync();
        return Ok(result);
    }

    // ============================================================
    // LEGACY COMPATIBILITY - OPTIONAL
    // Keep only if old frontend still calls these routes.
    // Remove later after frontend migration is complete.
    // ============================================================

    [HttpGet("get-all")]
    public async Task<IActionResult> LegacyGetInventory([FromQuery] InventoryQueryParams query)
    {
        var result = await _inventoryService.GetInventoryAsync(query);
        return Ok(result);
    }

    [HttpGet("get-categories")]
    public async Task<IActionResult> LegacyGetCategories([FromQuery] CategoryQueryParams query)
    {
        var result = await _inventoryService.GetCategoriesAsync(query);
        return Ok(result);
    }

    [HttpGet("get-leaf-categories")]
    public async Task<IActionResult> LegacyGetLeafCategories()
    {
        var result = await _inventoryService.GetLeafCategories();
        return Ok(result);
    }

    [HttpGet("get-leaf-categories-dropdown")]
    public async Task<IActionResult> LegacyGetLeafCategoriesDropdown()
    {
        var result = await _inventoryService.GetLeafCategoriesAsync();
        return Ok(result);
    }

    [HttpGet("item/{id:guid}")]
    public async Task<IActionResult> LegacyGetInventoryItemById(Guid id)
    {
        var result = await _inventoryService.GetInventoryItemById(id);
        return Ok(result);
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue("userId");

        if (!Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("User id missing.");

        return userId;
    }
}