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
    public async Task<IActionResult> GetAllCategory()
    {
        var result = await _commonService.GetAllCategoryAsync();
        return Ok(result);
    }

    [HttpPost("addCategory")]
    public async Task<IActionResult> AddCategory([FromBody] CategoryRequest request)
    {
        var result = await _commonService.AddCategoryAsync(request);
        return CreatedAtAction(nameof(GetAllCategory), new { id = result.Id }, result);
    }
}
