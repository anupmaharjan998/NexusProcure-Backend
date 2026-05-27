namespace NexusProcure.Core.DTOs.Dashboard;

public class DashboardPermissionDto
{
    public bool CanViewDashboard { get; set; }

    public bool CanViewEmployeeDashboard { get; set; }
    public bool CanViewMyRequisitionStats { get; set; }
    public bool CanViewMyAssignedItems { get; set; }

    public bool CanViewManagerDashboard { get; set; }
    public bool CanViewDepartmentRequisitionStats { get; set; }
    public bool CanViewPendingApprovalStats { get; set; }
    public bool CanViewDepartmentInventoryStats { get; set; }

    public bool CanViewProcurementDashboard { get; set; }
    public bool CanViewProcurementQueueStats { get; set; }
    public bool CanViewRfqStats { get; set; }
    public bool CanViewQuotationStats { get; set; }
    public bool CanViewPurchaseOrderStats { get; set; }
    public bool CanViewRecentPurchaseOrders { get; set; }
    public bool CanViewTodayDeliveries { get; set; }

    public bool CanViewInventoryDashboard { get; set; }
    public bool CanViewStockStats { get; set; }
    public bool CanViewLowStockAlerts { get; set; }
    public bool CanViewInventoryAssignmentStats { get; set; }
    public bool CanViewReceivingStats { get; set; }

    public bool CanViewFinanceDashboard { get; set; }
    public bool CanViewPurchaseCostStats { get; set; }
    public bool CanViewBudgetStats { get; set; }

    public bool CanViewExecutiveDashboard { get; set; }
    public bool CanViewExecutiveProcurementStats { get; set; }
    public bool CanViewDashboardCharts { get; set; }
    public bool CanViewDashboardAlerts { get; set; }

    public bool CanViewAdminDashboard { get; set; }
    public bool CanViewSystemStats { get; set; }

    public bool CanViewDashboardReports { get; set; }
    public bool CanExportDashboardReports { get; set; }
    public bool CanViewDashboardQuickActions { get; set; }
}