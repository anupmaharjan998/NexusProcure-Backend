using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Approval;

namespace NexusProcure.Api.Controllers;

public class TotalAmountRiskScoreController : BaseApiController
{
    private readonly ITotalAmountRiskScoreService _service;

    public TotalAmountRiskScoreController(ITotalAmountRiskScoreService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
        => Ok(await _service.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "ADD_TOTAL_AMOUNT_RISK_SCORE")]
    public async Task<IActionResult> Create(
        [FromBody] TotalAmountRiskScoreDto dto)
    {
        var id = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, null);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "UPDATE_TOTAL_AMOUNT_RISK_SCORE")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] TotalAmountRiskScoreDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "DELETE_TOTAL_AMOUNT_RISK_SCORE")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
