using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Vendor;
using NexusProcure.Core.Enums;

namespace NexusProcure.Api.Controllers;

public class VendorsController : BaseApiController
{
    private readonly IVendorService _service;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VendorsController(IVendorService service, IHttpContextAccessor httpContextAccessor)
    {
        _service = service;
        _httpContextAccessor = httpContextAccessor;
    }

    // CREATE VENDOR
    [HttpPost("add-vendor")]
    [Authorize(Policy = "ADD_VENDOR")]
    public async Task<IActionResult> Create([FromBody] VendorRequestDto dto)
    {
        var created = await _service.CreateVendorAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    // UPDATE VENDOR
    [HttpPut("{id}")]
    [Authorize(Policy = "EDIT_VENDOR")]
    public async Task<IActionResult> Update(Guid id, VendorRequestDto dto)
    {
        var updated = await _service.UpdateVendorAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    // VIEW SINGLE VENDOR
    [HttpGet("{id}")]
    [Authorize(Policy = "VIEW_VENDOR")]
    public async Task<IActionResult> Get(Guid id)
    {
        var v = await _service.GetVendorByIdAsync(id);
        if (v == null) return NotFound();
        return Ok(v);
    }

    // LIST VENDORS
    [HttpGet]
    [Authorize(Policy = "VIEW_VENDOR")]
    public async Task<IActionResult> List([FromQuery] string? status, [FromQuery] string? search)
    {
        var list = await _service.GetAllVendorsAsync(status, search);
        return Ok(list);
    }

    // APPROVE VENDOR
    [HttpPatch("{id}/status")]
    [Authorize(Policy = "APPROVE_VENDOR")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        var ok = await _service.UpdateVendorStatusAsync(id, status);
        if (!ok) return NotFound();
        return Ok();
    }

    // DELETE VENDOR
    [HttpDelete("{id}")]
    [Authorize(Policy = "DELETE_VENDOR")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _service.DeleteVendorAsync(id);
        if (!ok) return NotFound();
        return NoContent();
    }

    // UPLOAD VENDOR DOCUMENT
    [HttpPost("{id}/documents")]
    [Authorize(Policy = "UPLOAD_VENDOR_DOCUMENT")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocument(Guid id,[FromForm]  IFormFile file)
    {
        var userId = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        var doc = await _service.UploadVendorDocumentAsync(id, file, userId);
        return Ok(new { documentId = doc.Id, url = doc.FileUrl, fileName = doc.FileName });
    }

    // DELETE DOCUMENT
    [HttpDelete("documents/{docId}")]
    [Authorize(Policy = "DELETE_VENDOR_DOCUMENT")]
    public async Task<IActionResult> DeleteDocument(Guid docId)
    {
        var ok = await _service.DeleteVendorDocumentAsync(docId);
        if (!ok) return NotFound();
        return NoContent();
    }
    
    
    [HttpGet("get-payment-terms")]
    [Authorize]
    public ActionResult<IEnumerable<PaymentTermDto>> GetPaymentTerms()
    {
        var paymentTerms = Enum.GetValues(typeof(PaymentTerm))
            .Cast<PaymentTerm>()
            .Select(pt => new PaymentTermDto
            {
                Value = (int)pt,
                DisplayName = pt.GetDisplayName()
            })
            .ToList();

        return Ok(paymentTerms);
    }
}
