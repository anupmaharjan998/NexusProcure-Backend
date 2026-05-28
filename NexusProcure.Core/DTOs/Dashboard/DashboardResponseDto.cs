namespace NexusProcure.Core.DTOs.Dashboard;

public class DashboardResponseDto
{
    public DashboardPermissionDto Permissions { get; set; } = new();

    public SystemDashboardStatsDto? SystemStats { get; set; }
    public EmployeeDashboardStatsDto? EmployeeStats { get; set; }
    public ManagerDashboardStatsDto? ManagerStats { get; set; }
    public ProcurementDashboardStatsDto? ProcurementStats { get; set; }
    public InventoryDashboardStatsDto? InventoryStats { get; set; }
    public FinanceDashboardStatsDto? FinanceStats { get; set; }
    public ExecutiveDashboardStatsDto? ExecutiveStats { get; set; }

    public List<RecentPurchaseOrderDto> RecentPOs { get; set; } = new();
    public List<DeliveryDto> Deliveries { get; set; } = new();
    public List<DashboardQuickActionDto> QuickActions { get; set; } = new();
    public List<DashboardChartItemDto> ChartData { get; set; } = new();
    public List<DashboardAlertDto> Alerts { get; set; } = new();
    
    public List<AssignedInventoryPreviewDto> MyAssignedItems { get; set; } = new();
    public List<MyRequisitionPreviewDto> MyRecentRequisitions { get; set; } = new();
    public List<LowStockPreviewDto> LowStockItems { get; set; } = new();
    public List<PendingApprovalPreviewDto> PendingApprovals { get; set; } = new();
}