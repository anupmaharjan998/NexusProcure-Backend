using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.Constants;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Dashboard;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class DashboardService(NexusProcureDbContext context) : IDashboardService
{
    // public async Task<DashboardStatsDto> GetStatsAsync()
    // {
    //     // BASIC (single query)
    //     var basicStats = await context.Users
    //         .GroupBy(_ => 1)
    //         .Select(g => new
    //         {
    //             TotalUsers = context.Users.Count(x => x.IsActive),
    //             TotalDepartments = context.Departments.Count(),
    //             TotalRoles = context.Roles.Count(),
    //             TotalPermissions = context.Permissions.Count(),
    //             TotalVendors = context.Vendors.Count()
    //         })
    //         .FirstOrDefaultAsync();
    //
    //     // INVENTORY (single query)
    //     var inventoryStats = await context.InventoryItems
    //         .GroupBy(_ => 1)
    //         .Select(g => new
    //         {
    //             TotalInventoryItems = context.InventoryItems.Count(),
    //
    //             TotalStockQuantity = context.Stocks
    //                 .Sum(x => (int?)x.AvailableQuantity) ?? 0,
    //
    //             LowStockItems = context.InventoryItems
    //                 .Count(i => i.Stock != null &&
    //                             i.Stock.AvailableQuantity <= i.ReorderLevel),
    //
    //             AssignedItems = context.InventoryAssignments
    //                 .Where(x => !x.IsReturned)
    //                 .Sum(x => (int?)x.Quantity) ?? 0
    //         })
    //         .FirstOrDefaultAsync();
    //
    //     // PROCUREMENT (single query)
    //     var procurementStats = await context.PurchaseOrders
    //         .GroupBy(_ => 1)
    //         .Select(g => new
    //         {
    //             TotalRequisitions = context.Requisitions.Count(),
    //
    //             PendingRequisitions = context.Requisitions
    //                 .Count(x => x.Status == "Pending"),
    //
    //             ApprovedRequisitions = context.Requisitions
    //                 .Count(x => x.Status == "Approved"),
    //
    //             TotalPurchaseOrders = context.PurchaseOrders.Count(),
    //
    //             PendingPurchaseOrders = context.PurchaseOrders
    //                 .Count(x => x.Status == PurchaseOrderStatus.Issued),
    //
    //             CompletedPurchaseOrders = context.PurchaseOrders
    //                 .Count(x => x.Status == PurchaseOrderStatus.Completed),
    //
    //             ActiveProcurements = context.PurchaseOrders
    //                 .Count(x => x.Status != PurchaseOrderStatus.Completed)
    //         })
    //         .FirstOrDefaultAsync();
    //
    //     return new DashboardStatsDto
    //     {
    //         // BASIC
    //         TotalUsers = basicStats.TotalUsers,
    //         TotalDepartments = basicStats.TotalDepartments,
    //         TotalRoles = basicStats.TotalRoles,
    //         TotalPermissions = basicStats.TotalPermissions,
    //         TotalVendors = basicStats.TotalVendors,
    //
    //         // INVENTORY
    //         TotalInventoryItems = inventoryStats?.TotalInventoryItems ?? 0,
    //         TotalStockQuantity = inventoryStats?.TotalStockQuantity ?? 0,
    //         LowStockItems = inventoryStats?.LowStockItems ?? 0,
    //         AssignedItems = inventoryStats?.AssignedItems ?? 0,
    //
    //         // PROCUREMENT
    //         TotalRequisitions = procurementStats.TotalRequisitions,
    //         PendingRequisitions = procurementStats.PendingRequisitions,
    //         ApprovedRequisitions = procurementStats.ApprovedRequisitions,
    //         TotalPurchaseOrders = procurementStats.TotalPurchaseOrders,
    //         PendingPurchaseOrders = procurementStats.PendingPurchaseOrders,
    //         CompletedPurchaseOrders = procurementStats.CompletedPurchaseOrders,
    //         ActiveProcurements = procurementStats.ActiveProcurements
    //     };
    // }
    
    // public async Task<DashboardResponseDto> GetDashboardAsync()
    // {
    //     var stats = await context
    //         .Set<DashboardStatsDto>()
    //         .FromSqlRaw("SELECT * FROM get_dashboard_stats()")
    //         .AsNoTracking()
    //         .FirstOrDefaultAsync();
    //
    //     var pos = await context
    //         .Set<RecentPurchaseOrderDto>()
    //         .FromSqlRaw("SELECT * FROM get_recent_purchase_orders()")
    //         .AsNoTracking()
    //         .ToListAsync();
    //
    //     var deliveries = await context
    //         .Set<DeliveryDto>()
    //         .FromSqlRaw("SELECT * FROM get_today_deliveries()")
    //         .AsNoTracking()
    //         .ToListAsync();
    //
    //     return new DashboardResponseDto
    //     {
    //         Stats = stats ?? new DashboardStatsDto(),
    //         RecentPOs = pos,
    //         Deliveries = deliveries
    //     };
    // }
    
    public async Task<DashboardResponseDto> GetDashboardAsync(Guid userId)
        {
            var permissionKeys = await GetUserPermissionsAsync(userId);
            var permissions = BuildPermissionDto(permissionKeys);
    
            if (!permissions.CanViewDashboard)
                return new DashboardResponseDto { Permissions = permissions };
    
            var response = new DashboardResponseDto
            {
                Permissions = permissions
            };
    
            var user = await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == userId);
    
            if (user == null)
                return response;
    
            if (permissions.CanViewSystemStats)
                response.SystemStats = await GetSystemStatsAsync();
    
            if (permissions.CanViewEmployeeDashboard ||
                permissions.CanViewMyRequisitionStats ||
                permissions.CanViewMyAssignedItems)
                response.EmployeeStats = await GetEmployeeStatsAsync(userId);
            
            if (permissions.CanViewMyAssignedItems)
                response.MyAssignedItems = await GetMyAssignedItemsAsync(userId);

            if (permissions.CanViewMyRequisitionStats)
                response.MyRecentRequisitions = await GetMyRecentRequisitionsAsync(userId);
    
            if (permissions.CanViewManagerDashboard ||
                permissions.CanViewDepartmentRequisitionStats ||
                permissions.CanViewPendingApprovalStats ||
                permissions.CanViewDepartmentInventoryStats)
                response.ManagerStats = await GetManagerStatsAsync(user.DepartmentId);
            
            if (permissions.CanViewPendingApprovalStats)
                response.PendingApprovals = await GetPendingApprovalsAsync(user.DepartmentId);
    
            if (permissions.CanViewProcurementDashboard ||
                permissions.CanViewProcurementQueueStats ||
                permissions.CanViewRfqStats ||
                permissions.CanViewQuotationStats ||
                permissions.CanViewPurchaseOrderStats)
                response.ProcurementStats = await GetProcurementStatsAsync();
    
            if (permissions.CanViewInventoryDashboard ||
                permissions.CanViewStockStats ||
                permissions.CanViewLowStockAlerts ||
                permissions.CanViewInventoryAssignmentStats ||
                permissions.CanViewReceivingStats)
                response.InventoryStats = await GetInventoryStatsAsync();
            
            if (permissions.CanViewLowStockAlerts)
                response.LowStockItems = await GetLowStockItemsAsync();
    
            if (permissions.CanViewFinanceDashboard ||
                permissions.CanViewPurchaseCostStats ||
                permissions.CanViewBudgetStats)
                response.FinanceStats = await GetFinanceStatsAsync();
    
            if (permissions.CanViewExecutiveDashboard ||
                permissions.CanViewExecutiveProcurementStats)
                response.ExecutiveStats = await GetExecutiveStatsAsync();
    
            if (permissions.CanViewRecentPurchaseOrders)
                response.RecentPOs = await GetRecentPurchaseOrdersAsync();
    
            if (permissions.CanViewTodayDeliveries)
                response.Deliveries = await GetTodayDeliveriesAsync();
    
            if (permissions.CanViewDashboardCharts)
                response.ChartData = await GetChartDataAsync();
    
            if (permissions.CanViewDashboardAlerts)
                response.Alerts = await GetAlertsAsync();
    
            if (permissions.CanViewDashboardQuickActions)
                response.QuickActions = BuildQuickActions(permissionKeys);
    
            return response;
        }
    
        private async Task<HashSet<string>> GetUserPermissionsAsync(Guid userId)
        {
            var keys = await context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .SelectMany(u => u.Role.RolePermissions.Select(rp => rp.Permission.Key))
                .Distinct()
                .ToListAsync();
    
            return keys.ToHashSet();
        }
    
        private static bool Has(HashSet<string> permissions, string key)
        {
            return permissions.Contains(key);
        }
    
        private static DashboardPermissionDto BuildPermissionDto(HashSet<string> permissions)
        {
            return new DashboardPermissionDto
            {
                CanViewDashboard = Has(permissions, DashboardPermissions.ViewDashboard),
    
                CanViewEmployeeDashboard = Has(permissions, DashboardPermissions.ViewEmployeeDashboard),
                CanViewMyRequisitionStats = Has(permissions, DashboardPermissions.ViewMyRequisitionStats),
                CanViewMyAssignedItems = Has(permissions, DashboardPermissions.ViewMyAssignedItems),
    
                CanViewManagerDashboard = Has(permissions, DashboardPermissions.ViewManagerDashboard),
                CanViewDepartmentRequisitionStats = Has(permissions, DashboardPermissions.ViewDepartmentRequisitionStats),
                CanViewPendingApprovalStats = Has(permissions, DashboardPermissions.ViewPendingApprovalStats),
                CanViewDepartmentInventoryStats = Has(permissions, DashboardPermissions.ViewDepartmentInventoryStats),
    
                CanViewProcurementDashboard = Has(permissions, DashboardPermissions.ViewProcurementDashboard),
                CanViewProcurementQueueStats = Has(permissions, DashboardPermissions.ViewProcurementQueueStats),
                CanViewRfqStats = Has(permissions, DashboardPermissions.ViewRfqStats),
                CanViewQuotationStats = Has(permissions, DashboardPermissions.ViewQuotationStats),
                CanViewPurchaseOrderStats = Has(permissions, DashboardPermissions.ViewPurchaseOrderStats),
                CanViewRecentPurchaseOrders = Has(permissions, DashboardPermissions.ViewRecentPurchaseOrders),
                CanViewTodayDeliveries = Has(permissions, DashboardPermissions.ViewTodayDeliveries),
    
                CanViewInventoryDashboard = Has(permissions, DashboardPermissions.ViewInventoryDashboard),
                CanViewStockStats = Has(permissions, DashboardPermissions.ViewStockStats),
                CanViewLowStockAlerts = Has(permissions, DashboardPermissions.ViewLowStockAlerts),
                CanViewInventoryAssignmentStats = Has(permissions, DashboardPermissions.ViewInventoryAssignmentStats),
                CanViewReceivingStats = Has(permissions, DashboardPermissions.ViewReceivingStats),
    
                CanViewFinanceDashboard = Has(permissions, DashboardPermissions.ViewFinanceDashboard),
                CanViewPurchaseCostStats = Has(permissions, DashboardPermissions.ViewPurchaseCostStats),
                CanViewBudgetStats = Has(permissions, DashboardPermissions.ViewBudgetStats),
    
                CanViewExecutiveDashboard = Has(permissions, DashboardPermissions.ViewExecutiveDashboard),
                CanViewExecutiveProcurementStats = Has(permissions, DashboardPermissions.ViewExecutiveProcurementStats),
                CanViewDashboardCharts = Has(permissions, DashboardPermissions.ViewDashboardCharts),
                CanViewDashboardAlerts = Has(permissions, DashboardPermissions.ViewDashboardAlerts),
    
                CanViewAdminDashboard = Has(permissions, DashboardPermissions.ViewAdminDashboard),
                CanViewSystemStats = Has(permissions, DashboardPermissions.ViewSystemStats),
    
                CanViewDashboardReports = Has(permissions, DashboardPermissions.ViewDashboardReports),
                CanExportDashboardReports = Has(permissions, DashboardPermissions.ExportDashboardReports),
                CanViewDashboardQuickActions = Has(permissions, DashboardPermissions.ViewDashboardQuickActions)
            };
        }
    
        private async Task<SystemDashboardStatsDto> GetSystemStatsAsync()
        {
            return new SystemDashboardStatsDto
            {
                TotalUsers = await context.Users.CountAsync(x => x.IsActive),
                TotalDepartments = await context.Departments.CountAsync(),
                TotalRoles = await context.Roles.CountAsync(),
                TotalPermissions = await context.Permissions.CountAsync(),
                TotalVendors = await context.Vendors.CountAsync()
            };
        }
    
        private async Task<EmployeeDashboardStatsDto> GetEmployeeStatsAsync(Guid userId)
        {
            return new EmployeeDashboardStatsDto
            {
                MyTotalRequisitions = await context.Requisitions.CountAsync(x => x.RequestedById == userId),
                MyPendingRequisitions = await context.Requisitions.CountAsync(x => x.RequestedById == userId && x.Status == "Pending"),
                MyApprovedRequisitions = await context.Requisitions.CountAsync(x => x.RequestedById == userId && x.Status == "Approved"),
                MyRejectedRequisitions = await context.Requisitions.CountAsync(x => x.RequestedById == userId && x.Status == "Rejected"),
    
                MyAssignedItems = await context.InventoryAssignments
                    .Where(x => x.UserId == userId && !x.IsReturned)
                    .SumAsync(x => (int?)x.Quantity) ?? 0
            };
        }
    
        private async Task<ManagerDashboardStatsDto> GetManagerStatsAsync(Guid? departmentId)
        {
            if (departmentId == null)
                return new ManagerDashboardStatsDto();
    
            return new ManagerDashboardStatsDto
            {
                // DepartmentTotalRequisitions = await context.Requisitions.CountAsync(x => x.DepartmentId == departmentId),
                // DepartmentPendingRequisitions = await context.Requisitions.CountAsync(x => x.DepartmentId == departmentId && x.Status == "Pending"),
                // DepartmentApprovedRequisitions = await context.Requisitions.CountAsync(x => x.DepartmentId == departmentId && x.Status == "Approved"),
                // DepartmentRejectedRequisitions = await context.Requisitions.CountAsync(x => x.DepartmentId == departmentId && x.Status == "Rejected"),
                
                //todo
                DepartmentTotalRequisitions = 0,
                DepartmentPendingRequisitions = 0,
                DepartmentApprovedRequisitions = 0,
                DepartmentRejectedRequisitions = 0,
    
                PendingRequisitionApprovals = await context.Approvals
                    .CountAsync(x => x.Status == "Pending"),
    
                PendingQuotationApprovals = await context.Approvals
                    .CountAsync(x => x.Status == "Pending" && x.ReferenceType == ApprovalReferenceType.RFQ && x.IsActive),
    
                // DepartmentAssignedItems = await context.InventoryAssignments
                //     .Where(x => x.AssignedTo.DepartmentId == departmentId && !x.IsReturned)
                //     .SumAsync(x => (int?)x.Quantity) ?? 0
                    
                DepartmentAssignedItems = 0
            };
        }
    
        private async Task<ProcurementDashboardStatsDto> GetProcurementStatsAsync()
        {
            return new ProcurementDashboardStatsDto
            {
                TotalRequisitions = await context.Requisitions.CountAsync(),
    
                ApprovedWaitingForProcurement = await context.Requisitions
                    .CountAsync(x => x.Status == "Approved" || x.Status == "SentForProcurement"),
    
                TotalRfqs = await context.RequestForQuotations.CountAsync(),
    
                TotalQuotations = await context.Quotations.CountAsync(),
    
                PendingQuotationApprovals = await context.Approvals
                    .CountAsync(x => x.Status == "Pending" && x.ReferenceType == ApprovalReferenceType.RFQ && x.IsActive ),
    
                TotalPurchaseOrders = await context.PurchaseOrders.CountAsync(),
    
                ActivePurchaseOrders = await context.PurchaseOrders
                    .CountAsync(x => x.Status != PurchaseOrderStatus.Completed),
    
                CompletedPurchaseOrders = await context.PurchaseOrders
                    .CountAsync(x => x.DeliveryStatus == DeliveryStatus.Received),
    
                PartiallyReceivedPurchaseOrders = await context.PurchaseOrders
                    .CountAsync(x => x.DeliveryStatus == DeliveryStatus.PartiallyReceived),
    
                TodayDeliveries = await context.PurchaseOrders
                    .CountAsync(x => x.DeliveryDate.HasValue &&
                                     x.DeliveryDate.Value.Date == DateTime.UtcNow.Date)
            };
        }
    
        private async Task<InventoryDashboardStatsDto> GetInventoryStatsAsync()
        {
            return new InventoryDashboardStatsDto
            {
                TotalInventoryItems = await context.InventoryItems.CountAsync(),
    
                TotalStockQuantity = await context.InventoryStocks
                    .SumAsync(x => (int?)x.QuantityAvailable) ?? 0,
    
                LowStockItems = await context.InventoryItems
                    .CountAsync(i => i.Stock != null &&
                                     i.Stock.QuantityAvailable <= i.Stock.ReorderLevel),
    
                AssignedItems = await context.InventoryAssignments
                    .Where(x => !x.IsReturned)
                    .SumAsync(x => (int?)x.Quantity) ?? 0,
    
                ItemsToReceive = await context.PurchaseOrders
                    .CountAsync(x => x.Status == PurchaseOrderStatus.Issued ||
                                     x.DeliveryStatus == DeliveryStatus.PartiallyReceived),
    
                ReturnedItems = await context.InventoryAssignments
                    .CountAsync(x => x.IsReturned),
    
                DamagedItems = await context.InventoryItems
                    .CountAsync(x => x.Condition == InventoryItemCondition.Damaged)
            };
        }
    
        private async Task<FinanceDashboardStatsDto> GetFinanceStatsAsync()
        {
            return new FinanceDashboardStatsDto
            {
                TotalPurchaseValue = await context.PurchaseOrders
                    .SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
    
                ActivePurchaseValue = await context.PurchaseOrders
                    .Where(x => x.Status != PurchaseOrderStatus.Completed)
                    .SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
    
                CompletedPurchaseValue = await context.PurchaseOrders
                    .Where(x => x.Status == PurchaseOrderStatus.Completed)
                    .SumAsync(x => (decimal?)x.TotalAmount) ?? 0,
    
                PendingApprovalValue = await context.RequestForQuotations
                    .Where(x => x.Status == RfqStatus.PendingApproval)
                    .SumAsync(x => (decimal?)x.Requisition.TotalAmount) ?? 0
            };
        }
    
        private async Task<ExecutiveDashboardStatsDto> GetExecutiveStatsAsync()
        {
            return new ExecutiveDashboardStatsDto
            {
                TotalDepartments = await context.Departments.CountAsync(),
                TotalVendors = await context.Vendors.CountAsync(),
                TotalRequisitions = await context.Requisitions.CountAsync(),
    
                ActivePurchaseOrders = await context.PurchaseOrders
                    .CountAsync(x => x.Status != PurchaseOrderStatus.Completed),
    
                CompletedPurchaseOrders = await context.PurchaseOrders
                    .CountAsync(x => x.Status == PurchaseOrderStatus.Completed),
    
                PendingApprovals = await context.Approvals.CountAsync(x => x.Status == "Pending" && x.IsActive),
    
                LowStockItems = await context.InventoryItems
                    .CountAsync(i => i.Stock != null &&
                                     i.Stock.QuantityAvailable <= i.Stock.ReorderLevel),
    
                TotalPurchaseValue = await context.PurchaseOrders
                    .SumAsync(x => (decimal?)x.TotalAmount) ?? 0
            };
        }
    
        private async Task<List<RecentPurchaseOrderDto>> GetRecentPurchaseOrdersAsync()
        {
            return await context.PurchaseOrders
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(6)
                .Select(x => new RecentPurchaseOrderDto
                {
                    Id = x.Id,
                    PoNumber = x.PurchaseOrderNumber,
                    VendorName = x.Vendor.CompanyName,
                    TotalAmount = x.TotalAmount,
                    TotalItems = x.Items.Count,
                    Status = (int)x.Status
                })
                .ToListAsync();
        }
    
        private async Task<List<DeliveryDto>> GetTodayDeliveriesAsync()
        {
            var today = DateTime.UtcNow.Date;
    
            return await context.PurchaseOrders
                .AsNoTracking()
                .Where(x => x.DeliveryDate.HasValue &&
                            x.DeliveryDate.Value.Date == today)
                .OrderBy(x => x.DeliveryDate)
                .Select(x => new DeliveryDto
                {
                    Id = x.Id,
                    PoNumber = x.PurchaseOrderNumber,
                    VendorName = x.Vendor.CompanyName,
                    TotalItems = x.Items.Count,
                    ExpectedDeliveryDate = x.DeliveryDate
                })
                .ToListAsync();
        }
    
        private async Task<List<DashboardChartItemDto>> GetChartDataAsync()
        {
            return new List<DashboardChartItemDto>
            {
                new()
                {
                    Name = "Requisitions",
                    Value = await context.Requisitions.CountAsync()
                },
                new()
                {
                    Name = "Quotations",
                    Value = await context.Quotations.CountAsync()
                },
                new()
                {
                    Name = "Purchase Orders",
                    Value = await context.PurchaseOrders.CountAsync()
                },
                new()
                {
                    Name = "Low Stock",
                    Value = await context.InventoryItems
                        .CountAsync(i => i.Stock != null &&
                                         i.Stock.QuantityAvailable <= i.Stock.ReorderLevel)
                }
            };
        }
    
        private async Task<List<DashboardAlertDto>> GetAlertsAsync()
        {
            var alerts = new List<DashboardAlertDto>();
    
            var lowStockCount = await context.InventoryItems
                .CountAsync(i => i.Stock != null &&
                                 i.Stock.QuantityAvailable <= i.Stock.ReorderLevel);
    
            if (lowStockCount > 0)
            {
                alerts.Add(new DashboardAlertDto
                {
                    Title = "Low stock alert",
                    Message = $"{lowStockCount} inventory item(s) are below reorder level.",
                    Severity = "warning",
                    Path = "/inventory"
                });
            }
    
            var pendingApprovals = await context.Approvals.CountAsync(x => x.Status == "Pending");
    
            if (pendingApprovals > 0)
            {
                alerts.Add(new DashboardAlertDto
                {
                    Title = "Pending approvals",
                    Message = $"{pendingApprovals} requisition approval(s) are pending.",
                    Severity = "info",
                    Path = "/procurement/requisitions-approvals"
                });
            }
    
            var todayDeliveries = await context.PurchaseOrders
                .CountAsync(x => x.DeliveryDate.HasValue &&
                                 x.DeliveryDate.Value.Date == DateTime.UtcNow.Date);
    
            if (todayDeliveries > 0)
            {
                alerts.Add(new DashboardAlertDto
                {
                    Title = "Deliveries today",
                    Message = $"{todayDeliveries} purchase order delivery/deliveries scheduled today.",
                    Severity = "success",
                    Path = "/procurement/purchase-orders"
                });
            }
    
            return alerts;
        }
    
        private static List<DashboardQuickActionDto> BuildQuickActions(HashSet<string> permissions)
        {
            var actions = new List<DashboardQuickActionDto>();
    
            if (Has(permissions, "CREATE_REQUISITION"))
            {
                actions.Add(new DashboardQuickActionDto
                {
                    Label = "Create Requisition",
                    Path = "/requisitions/create",
                    Permission = "CREATE_REQUISITION"
                });
            }
    
            if (Has(permissions, "APPROVE_REQUISITION"))
            {
                actions.Add(new DashboardQuickActionDto
                {
                    Label = "Review Requisition Approvals",
                    Path = "/procurement/requisitions-approvals",
                    Permission = "APPROVE_REQUISITION"
                });
            }
    
            if (Has(permissions, "VIEW_PURCHASE_ORDER"))
            {
                actions.Add(new DashboardQuickActionDto
                {
                    Label = "Purchase Orders",
                    Path = "/procurement/purchase-orders",
                    Permission = "VIEW_PURCHASE_ORDER"
                });
            }
    
            if (Has(permissions, "VIEW_INVENTORY"))
            {
                actions.Add(new DashboardQuickActionDto
                {
                    Label = "Inventory",
                    Path = "/inventory",
                    Permission = "VIEW_INVENTORY"
                });
            }
    
            if (Has(permissions, "VIEW_VENDOR"))
            {
                actions.Add(new DashboardQuickActionDto
                {
                    Label = "Vendors",
                    Path = "/vendors",
                    Permission = "VIEW_VENDOR"
                });
            }
    
            if (Has(permissions, "VIEW_REPORTS"))
            {
                actions.Add(new DashboardQuickActionDto
                {
                    Label = "Reports",
                    Path = "/reports",
                    Permission = "VIEW_REPORTS"
                });
            }
    
            return actions;
        }
        
        private async Task<List<AssignedInventoryPreviewDto>> GetMyAssignedItemsAsync(Guid userId)
{
    return await context.InventoryAssignments
        .AsNoTracking()
        .Where(x => x.UserId == userId && !x.IsReturned)
        .OrderByDescending(x => x.AssignedDate)
        .Take(5)
        .Select(x => new AssignedInventoryPreviewDto
        {
            AssignmentId = x.Id,
            InventoryItemId = x.InventoryItemId,
            ItemName = x.InventoryItem.Name,
            CategoryName = x.InventoryItem.InventoryCategory != null
                ? x.InventoryItem.InventoryCategory.Name
                : null,
            SerialNumber = x.InventoryItem.SerialNumber,
            Barcode = x.InventoryItem.Barcode,
            Location = x.InventoryItem.Location,
            Condition = x.InventoryItem.Condition.ToString(),
            Quantity = x.Quantity,
            AssignedAt = x.AssignedDate
        })
        .ToListAsync();
}

private async Task<List<MyRequisitionPreviewDto>> GetMyRecentRequisitionsAsync(Guid userId)
{
    return await context.Requisitions
        .AsNoTracking()
        .Where(x => x.RequestedById == userId)
        .OrderByDescending(x => x.RequestedDate)
        .Take(5)
        .Select(x => new MyRequisitionPreviewDto
        {
            RequisitionId = x.Id,
            RequisitionNumber = x.RequisitionNumber,
            Status = x.Status,
            TotalItems = x.Items.Count,
            CreatedAt = x.RequestedDate
        })
        .ToListAsync();
}

private async Task<List<LowStockPreviewDto>> GetLowStockItemsAsync()
{
    return await context.InventoryItems
        .AsNoTracking()
        .Where(x => x.Stock != null &&
                    x.Stock.QuantityAvailable <= x.Stock.ReorderLevel)
        .OrderBy(x => x.Stock!.QuantityAvailable)
        .Take(5)
        .Select(x => new LowStockPreviewDto
        {
            InventoryItemId = x.Id,
            ItemName = x.Name,
            CategoryName = x.InventoryCategory != null
                ? x.InventoryCategory.Name
                : null,
            AvailableQuantity = x.Stock != null ? x.Stock.QuantityAvailable : 0,
            ReorderLevel = x.Stock.ReorderLevel,
            Location = x.Location
        })
        .ToListAsync();
}

private async Task<List<PendingApprovalPreviewDto>> GetPendingApprovalsAsync(Guid? departmentId)
{
    var query = context.Approvals
        .AsNoTracking()
        .Where(x => x.Status == "Pending" && x.IsActive);
    

    if (departmentId.HasValue)
    {
        //query = query.Where(x => x.ReferenceType.DepartmentId == departmentId.Value);
    }

    return await query
        .OrderByDescending(x => x.AssignedAt)
        .Take(5)
        .Select(x => new PendingApprovalPreviewDto
        {
            ApprovalId = x.Id,
            RequisitionId = x.ReferenceId,
            // RequisitionNumber = x.Requisition.RequisitionNumber,
            // RequestedBy = x.Requisition.RequestedBy.FullName,
            // DepartmentName = x.Requisition.Department.Name,
            // Status = x.Status,
            // RequestedAt = x.Requisition.CreatedAt
        })
        .ToListAsync();
}
}