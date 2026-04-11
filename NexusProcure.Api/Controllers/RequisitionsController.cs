using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs.Procurement;

namespace NexusProcure.Api.Controllers;

public class RequisitionsController : BaseApiController
{
    private readonly IRequisitionService _requisitionService;

    public RequisitionsController(IRequisitionService requisitionService)
    {
        _requisitionService = requisitionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _requisitionService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _requisitionService.GetByIdAsync(id));

    [HttpPost]
    [Authorize(Policy = "CREATE_REQUISITION")]
    public async Task<IActionResult> Create(RequisitionCreateDto dto)
    {
        var requestedById = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        dto.RequestedById = requestedById;
        return Ok(await _requisitionService.CreateAsync(dto));
    } 

    [HttpPost("{id}/approve")]
    public async Task<IActionResult> Approve(Guid id, string comments)
    {
        var approvedById = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        return Ok(await _requisitionService.ApproveAsync(id, approvedById, comments));
    }

    [HttpPost("{id}/reject")]
    public async Task<IActionResult> Reject(Guid id, string comments)
    {
        var rejectedById = Guid.Parse(User.FindFirstValue("userId") ?? throw new Exception("user id missing"));
        return Ok(await _requisitionService.RejectAsync(id, rejectedById, comments));
    }
}