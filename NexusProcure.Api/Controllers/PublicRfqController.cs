using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.RequestForQuotation;
using NexusProcure.Core.DTOs.RFQ;

namespace NexusProcure.Api.Controllers;

[AllowAnonymous]
public class PublicRfqController : BaseApiController
{
    private readonly IRfqService _rfqService;
    private readonly IRfqExcelService _rfqExcelService;

    public PublicRfqController(IRfqService rfqService, IRfqExcelService rfqExcelService)
    {
        _rfqService = rfqService;
        _rfqExcelService = rfqExcelService;
    }

    [HttpGet("{token}")]
    public async Task<IActionResult> GetRfq(string token)
    {
        var rfq = await _rfqService.GetRfqByTokenAsync(token);

        if (rfq == null)
            return NotFound("Invalid or expired RFQ link");

        return Ok(rfq);
    }


    [HttpGet("validate/{token}")]
    public async Task<IActionResult> ValidateRfqToken(string token)
    {
        var rfq = await _rfqService.ValidateRfqTokenAsync(token);

        if (rfq == null)
            return NotFound("Invalid or used RFQ token");

        return Ok(rfq);
    }

    [HttpPost("{token}/submit")]
    public async Task<IActionResult> SubmitQuotation(string token, [FromBody] QuotationSubmitDto dto)
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
        await _rfqService.SubmitQuotationAsync(token, dto, ip);

        return Ok(new
        {
            message = "Quotation submitted successfully."
        });
    }
    
    [HttpGet("{token}/template")]
    [Produces("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")]
    public async Task<IActionResult> DownloadTemplate(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return BadRequest("Token is required.");

        var rfq = await _rfqService.GetRfqByTokenAsync(token);
        if (rfq == null)
            return Unauthorized("Invalid or expired RFQ token.");

        var fileBytes = _rfqExcelService.GenerateTemplate(rfq);

        var fileName = $"RFQ-{rfq.RfqNumber}-Quotation.xlsx";

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }

    
    
    [HttpPost("{token}/upload")]
    public async Task<IActionResult> UploadQuotationExcel(
        string token,
        [FromForm] QuotationExcelUploadDto dto)
    {
        await _rfqService.SubmitQuotationFromExcelAsync(token, dto.File);
        return Ok("Quotation uploaded successfully");
    }


}