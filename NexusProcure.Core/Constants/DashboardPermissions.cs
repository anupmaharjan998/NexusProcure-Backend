namespace NexusProcure.Core.Constants;

public static class DashboardPermissions
{
    public const string ViewDashboard = "VIEW_DASHBOARD";

    // Employee
    public const string ViewEmployeeDashboard = "VIEW_EMPLOYEE_DASHBOARD";
    public const string ViewMyRequisitionStats = "VIEW_MY_REQUISITION_STATS";
    public const string ViewMyAssignedItems = "VIEW_MY_ASSIGNED_ITEMS";

    // Manager
    public const string ViewManagerDashboard = "VIEW_MANAGER_DASHBOARD";
    public const string ViewDepartmentRequisitionStats = "VIEW_DEPARTMENT_REQUISITION_STATS";
    public const string ViewPendingApprovalStats = "VIEW_PENDING_APPROVAL_STATS";
    public const string ViewDepartmentInventoryStats = "VIEW_DEPARTMENT_INVENTORY_STATS";

    // Procurement
    public const string ViewProcurementDashboard = "VIEW_PROCUREMENT_DASHBOARD";
    public const string ViewProcurementQueueStats = "VIEW_PROCUREMENT_QUEUE_STATS";
    public const string ViewRfqStats = "VIEW_RFQ_STATS";
    public const string ViewQuotationStats = "VIEW_QUOTATION_STATS";
    public const string ViewPurchaseOrderStats = "VIEW_PURCHASE_ORDER_STATS";
    public const string ViewRecentPurchaseOrders = "VIEW_RECENT_PURCHASE_ORDERS";
    public const string ViewTodayDeliveries = "VIEW_TODAY_DELIVERIES";

    // Inventory
    public const string ViewInventoryDashboard = "VIEW_INVENTORY_DASHBOARD";
    public const string ViewStockStats = "VIEW_STOCK_STATS";
    public const string ViewLowStockAlerts = "VIEW_LOW_STOCK_ALERTS";
    public const string ViewInventoryAssignmentStats = "VIEW_INVENTORY_ASSIGNMENT_STATS";
    public const string ViewReceivingStats = "VIEW_RECEIVING_STATS";

    // Finance
    public const string ViewFinanceDashboard = "VIEW_FINANCE_DASHBOARD";
    public const string ViewPurchaseCostStats = "VIEW_PURCHASE_COST_STATS";
    public const string ViewBudgetStats = "VIEW_BUDGET_STATS";

    // CEO / Executive
    public const string ViewExecutiveDashboard = "VIEW_EXECUTIVE_DASHBOARD";
    public const string ViewExecutiveProcurementStats = "VIEW_EXECUTIVE_PROCUREMENT_STATS";
    public const string ViewDashboardCharts = "VIEW_DASHBOARD_CHARTS";
    public const string ViewDashboardAlerts = "VIEW_DASHBOARD_ALERTS";

    // Admin
    public const string ViewAdminDashboard = "VIEW_ADMIN_DASHBOARD";
    public const string ViewSystemStats = "VIEW_SYSTEM_STATS";

    // Reports / actions
    public const string ViewDashboardReports = "VIEW_DASHBOARD_REPORTS";
    public const string ExportDashboardReports = "EXPORT_DASHBOARD_REPORTS";
    public const string ViewDashboardQuickActions = "VIEW_DASHBOARD_QUICK_ACTIONS";
}