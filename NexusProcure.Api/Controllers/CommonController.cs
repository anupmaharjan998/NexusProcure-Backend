using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;

namespace NexusProcure.Api.Controllers;
[Authorize]
public class CommonController : BaseApiController
{
    private readonly ICommonService _commonService;

    public CommonController(ICommonService  commonService)
    {
        _commonService = commonService;
    }
    
    [HttpGet("getAllCategories")]
    [Authorize]
    public async Task<IActionResult> GetAllCategory()
    {
        var result = await _commonService.GetAllCategoryAsync();
        return Ok(result);
    }

    [HttpPost("addCategory")]
    [Authorize]
    public async Task<IActionResult> AddCategory([FromBody] CategoryRequest request)
    {
        var result = await _commonService.AddCategoryAsync(request);
        return CreatedAtAction(nameof(GetAllCategory), new { id = result.Id }, result);
    }
    
    [HttpGet("get-category/{id}")]
    [Authorize]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var role = await _commonService.GetByCategoryByIdAsync(id);
        return role == null ? NotFound() : Ok(role);
    }
    
    [HttpPut("{id}/update-category")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, CategoryRequest dto)
    {
        var updated = await _commonService.UpdateCategoryAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}/delete-category")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _commonService.DeleteCategoryAsync(id);
        return result ? NoContent() : NotFound();
    }
}
