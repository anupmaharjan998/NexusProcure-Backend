namespace NexusProcure.Core.DTOs.Reports;

public class DashboardSummaryDto
{
    public int TotalRequisitions { get; set; }
    public int PendingApprovals { get; set; }
    public int ApprovedRequisitions { get; set; }
    public int RejectedRequisitions { get; set; }

    public int TotalRfqs { get; set; }
    public int TotalPurchaseOrders { get; set; }
    public decimal TotalPoValue { get; set; }

    public int PendingDeliveries { get; set; }
    public int CompletedDeliveries { get; set; }
    public int LowStockItems { get; set; }
}