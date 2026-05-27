namespace NexusProcure.Core.DTOs.Reports;

public class ReportsDashboardDto
{
    public DashboardSummaryDto Summary { get; set; } = new();

    public List<ChartReportDto> RequisitionStatus { get; set; } = new();

    public List<MonthlySpendDto> MonthlySpend { get; set; } = new();

    public List<LowStockReportDto> LowStockItems { get; set; } = new();

    public List<PurchaseOrderReportDto> TodayDeliveries { get; set; } = new();

    public List<PurchaseOrderReportDto> OverdueDeliveries { get; set; } = new();
}