using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NexusProcure.Application.Interfaces.Reports;
using NexusProcure.Core.DTOs.Reports;

namespace NexusProcure.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("dashboard")]
    [Authorize(Policy = "VIEW_DASHBOARD_REPORTS")]
    public async Task<IActionResult> GetDashboard()
    {
        var result = await _reportService.GetDashboardAsync();
        return Ok(result);
    }

    [HttpGet("summary")]
    [Authorize(Policy = "VIEW_DASHBOARD_REPORTS")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _reportService.GetDashboardSummaryAsync();
        return Ok(result);
    }

    [HttpGet("requisition-status")]
    [Authorize(Policy = "VIEW_DASHBOARD_REPORTS")]
    public async Task<IActionResult> GetRequisitionStatus()
    {
        var result = await _reportService.GetRequisitionStatusAsync();
        return Ok(result);
    }

    [HttpGet("monthly-spend")]
    [Authorize(Policy = "VIEW_DASHBOARD_REPORTS")]
    public async Task<IActionResult> GetMonthlySpend([FromQuery] int? year)
    {
        var selectedYear = year ?? DateTime.UtcNow.Year;
        var result = await _reportService.GetMonthlySpendAsync(selectedYear);
        return Ok(result);
    }

    [HttpGet("purchase-orders")]
    public async Task<IActionResult> GetPurchaseOrders([FromQuery] PurchaseOrderReportQuery query)
    {
        var result = await _reportService.GetPurchaseOrderReportAsync(query);
        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock()
    {
        var result = await _reportService.GetLowStockReportAsync();
        return Ok(result);
    }
}