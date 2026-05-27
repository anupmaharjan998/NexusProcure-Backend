
using NexusProcure.Core.DTOs.Common;
using NexusProcure.Core.DTOs.Reports;

namespace NexusProcure.Application.Interfaces.Reports;


public interface IReportService
{
    Task<ReportsDashboardDto> GetDashboardAsync();

    Task<DashboardSummaryDto> GetDashboardSummaryAsync();

    Task<List<ChartReportDto>> GetRequisitionStatusAsync();

    Task<List<MonthlySpendDto>> GetMonthlySpendAsync(int year);

    Task<PagedResult<PurchaseOrderReportDto>> GetPurchaseOrderReportAsync(PurchaseOrderReportQuery query);

    Task<List<LowStockReportDto>> GetLowStockReportAsync();
}