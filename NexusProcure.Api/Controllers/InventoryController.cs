using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Api.Controllers;

public class InventoryController : BaseApiController
{
    private readonly IInventoryService _inventoryService;
    private readonly IInventoryCategoryService _inventoryCategoryService;
    private readonly IInventoryItemService _inventoryItemService;

    public InventoryController(IInventoryService inventoryService, IInventoryCategoryService inventoryCategoryService,
        IInventoryItemService inventoryItemService)
    {
        _inventoryService = inventoryService;
        _inventoryCategoryService = inventoryCategoryService;
        _inventoryItemService = inventoryItemService;
    }


    [HttpPost]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        await _inventoryCategoryService.CreateAsync(dto);
        return Ok();
    }

    [HttpGet("tree")]
    public async Task<IActionResult> GetTree()
    {
        return Ok(await _inventoryCategoryService.GetTreeAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateInventoryItemDto dto)
    {
        await _inventoryItemService.CreateAsync(dto);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CreateInventoryItemDto dto)
    {
        await _inventoryItemService.UpdateAsync(id, dto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _inventoryItemService.DeleteAsync(id);
        return Ok();
    }


    [HttpPost("receive/{poId}")]
    public async Task<IActionResult> Receive(Guid poId)
    {
        await _inventoryService.ReceiveFromPurchaseOrderAsync(poId);
        return Ok();
    }

    [HttpPost("assign/{requisitionId}")]
    public async Task<IActionResult> Assign(Guid requisitionId)
    {
        await _inventoryService.AssignToUserAsync(requisitionId);
        return Ok();
    }

    [HttpGet("get-all")]
    public async Task<IActionResult> GetInventory([FromQuery] InventoryQueryParams query)
    {
        return Ok(await _inventoryService.GetInventoryAsync(query));
    }


    #region Category

    [HttpGet("get-categories")]
    public async Task<IActionResult> GetCategories([FromQuery] CategoryQueryParams query)
    {
        var result = await _inventoryService.GetCategoriesAsync(query);
        return Ok(result);
    }

    [HttpPost("categories")]
    [Authorize(Policy = "ADD_CATEGORIES")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        await _inventoryService.CreateCategoryAsync(dto, userId);
        return Ok(new { message = "Category created successfully" });
    }

    [HttpPut("categories/{id}")]
    [Authorize(Policy = "UPDATE_CATEGORIES")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateInventoryCategoryDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        await _inventoryService.UpdateCategoryAsync(id, dto, userId);
        return Ok();
    }

    [HttpDelete("categories/{id}")]
    [Authorize(Policy = "DELETE_CATEGORIES")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        await _inventoryService.DeleteCategoryAsync(id);
        return Ok();
    }

    /* ---------------- Get Leaf Categories ---------------- */
    [HttpGet("get-leaf-categories-dropdown")]
    public async Task<IActionResult> GetLeafCategoriesForDropDown()
    {
        var result = await _inventoryService.GetLeafCategoriesAsync();
        return Ok(result);
    }

    /* ---------------- Get Items By Category ---------------- */
    [HttpGet("items/by-category/{categoryId}")]
    public async Task<IActionResult> GetItemsByCategory(Guid categoryId)
    {
        var result = await _inventoryService.GetItemsByCategoryAsync(categoryId);
        return Ok(result);
    }

    #endregion


    #region Inventory Item

    [HttpPost("create-item")]
    [Authorize(Policy = "ADD_INVENTORY_ITEM")]
    public async Task<IActionResult> CreateItem([FromBody] CreateInventoryItemDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        var result = await _inventoryService.CreateItemAsync(dto, userId);
        return Ok(result);
    }

    [HttpPut("update-item/{id}")]
    [Authorize(Policy = "UPDATE_INVENTORY_ITEM")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateInventoryItemDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        var result = await _inventoryService.UpdateItemAsync(id, dto, userId);
        return Ok(result);
    }

    [HttpGet("item/{id}")]
    public async Task<IActionResult> GetInventoryItemById(Guid id)
    {
        var result = await _inventoryService.GetInventoryItemById(id);
        return Ok(result);
    }


    [HttpGet("get-leaf-categories")]
    public async Task<IActionResult> GetLeafCategories()
    {
        var result = await _inventoryService.GetLeafCategories();
        return Ok(result);
    }

    [HttpGet("preview-sku")]
    public async Task<IActionResult> PreviewSku(string name, Guid categoryId)
    {
        var sku = await _inventoryService.GenerateSkuAsync(name, categoryId);
        return Ok(sku);
    }


    [HttpPost("{id:guid}/assign")]
    [Authorize(Policy = "ASSIGN_ASSET")]
    public async Task<IActionResult> AssignItem(Guid id, [FromBody] AssignInventoryItemDto dto)
    {
        var userIdClaim = User.FindFirstValue("userId");
        if (!Guid.TryParse(userIdClaim, out var assignedBy))
            return Unauthorized("User id missing.");

        var result = await _inventoryService.AssignItemAsync(id, dto, assignedBy);
        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpPost("{id:guid}/unassign")]
    [Authorize(Policy = "UNASSIGN_ASSET")]
    public async Task<IActionResult> UnassignItem(Guid id)
    {
        var userIdClaim = User.FindFirstValue("userId");
        if (!Guid.TryParse(userIdClaim, out var unassignedBy))
            return Unauthorized("User id missing.");

        var result = await _inventoryService.UnassignItemAsync(id, unassignedBy);
        if (result == null) return NotFound();

        return Ok(result);
    }

    #endregion
}