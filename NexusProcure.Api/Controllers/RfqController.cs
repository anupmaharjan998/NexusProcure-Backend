using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.RequestForQuotation;

namespace NexusProcure.Api.Controllers;

public class RfqController : BaseApiController
{
    private readonly IRfqService _rfqService;

    public RfqController(IRfqService rfqService)
    {
        _rfqService = rfqService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllRfq() => Ok(await _rfqService.GetAllRfqAsync());


    [HttpGet("{rfqId}/quotations")]
    public async Task<IActionResult> GetQuotations(Guid rfqId)
    {
        var result = await _rfqService.GetQuotationByRfqIdAsync(rfqId);
        return Ok(result);
    }

    [HttpGet("{quotationId}/quotations-details")]
    public async Task<IActionResult> GetQuotationsById(Guid quotationId)
    {
        var result = await _rfqService.GetQuotationByIdAsync(quotationId);
        return Ok(result);
    }


    [HttpPost("compare")]
    public async Task<IActionResult> Compare([FromBody] List<Guid> quotationIds)
    {
        if (quotationIds == null || quotationIds.Count < 2)
            return BadRequest("At least two quotations are required for comparison.");

        var result = await _rfqService.CompareQuotationsAsync(quotationIds);

        return Ok(result);
    }

    [HttpPost("{rfqId}/select-quotation")]
    public async Task<IActionResult> SelectQuotation(Guid rfqId, Guid quotationId)
    {
        await _rfqService.SelectQuotationAsync(rfqId, quotationId);
        return NoContent();
    }
    
    [HttpPost("{rfqId}/clear-selection")]
    public async Task<IActionResult> ClearSelection(Guid rfqId)
    {
        await _rfqService.ClearSelectedQuotationAsync(rfqId);
        return NoContent();
    }

}