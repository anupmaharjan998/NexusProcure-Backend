namespace NexusProcure.Core.DTOs.Dashboard;


public class SystemDashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalDepartments { get; set; }
    public int TotalRoles { get; set; }
    public int TotalPermissions { get; set; }
    public int TotalVendors { get; set; }
}

public class EmployeeDashboardStatsDto
{
    public int MyTotalRequisitions { get; set; }
    public int MyPendingRequisitions { get; set; }
    public int MyApprovedRequisitions { get; set; }
    public int MyRejectedRequisitions { get; set; }
    public int MyAssignedItems { get; set; }
}

public class ManagerDashboardStatsDto
{
    public int DepartmentTotalRequisitions { get; set; }
    public int DepartmentPendingRequisitions { get; set; }
    public int DepartmentApprovedRequisitions { get; set; }
    public int DepartmentRejectedRequisitions { get; set; }
    public int PendingRequisitionApprovals { get; set; }
    public int PendingQuotationApprovals { get; set; }
    public int DepartmentAssignedItems { get; set; }
}

public class ProcurementDashboardStatsDto
{
    public int TotalRequisitions { get; set; }
    public int ApprovedWaitingForProcurement { get; set; }
    public int TotalRfqs { get; set; }
    public int TotalQuotations { get; set; }
    public int PendingQuotationApprovals { get; set; }
    public int TotalPurchaseOrders { get; set; }
    public int ActivePurchaseOrders { get; set; }
    public int CompletedPurchaseOrders { get; set; }
    public int PartiallyReceivedPurchaseOrders { get; set; }
    public int TodayDeliveries { get; set; }
}

public class InventoryDashboardStatsDto
{
    public int TotalInventoryItems { get; set; }
    public int TotalStockQuantity { get; set; }
    public int LowStockItems { get; set; }
    public int AssignedItems { get; set; }
    public int ItemsToReceive { get; set; }
    public int ReturnedItems { get; set; }
    public int DamagedItems { get; set; }
}

public class FinanceDashboardStatsDto
{
    public decimal TotalPurchaseValue { get; set; }
    public decimal ActivePurchaseValue { get; set; }
    public decimal CompletedPurchaseValue { get; set; }
    public decimal PendingApprovalValue { get; set; }
}

public class ExecutiveDashboardStatsDto
{
    public int TotalDepartments { get; set; }
    public int TotalVendors { get; set; }
    public int TotalRequisitions { get; set; }
    public int ActivePurchaseOrders { get; set; }
    public int CompletedPurchaseOrders { get; set; }
    public int PendingApprovals { get; set; }
    public int LowStockItems { get; set; }
    public decimal TotalPurchaseValue { get; set; }
}

public class DashboardQuickActionDto
{
    public string Label { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public string Permission { get; set; } = string.Empty;
}

public class DashboardChartItemDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class DashboardAlertDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Severity { get; set; } = "info";
    public string? Path { get; set; }
}