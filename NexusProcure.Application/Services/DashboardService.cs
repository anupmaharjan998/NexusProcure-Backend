using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
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
    
    public async Task<DashboardResponseDto> GetDashboardAsync()
    {
        var stats = await context
            .Set<DashboardStatsDto>()
            .FromSqlRaw("SELECT * FROM get_dashboard_stats()")
            .AsNoTracking()
            .FirstOrDefaultAsync();

        var pos = await context
            .Set<RecentPurchaseOrderDto>()
            .FromSqlRaw("SELECT * FROM get_recent_purchase_orders()")
            .AsNoTracking()
            .ToListAsync();

        var deliveries = await context
            .Set<DeliveryDto>()
            .FromSqlRaw("SELECT * FROM get_today_deliveries()")
            .AsNoTracking()
            .ToListAsync();

        return new DashboardResponseDto
        {
            Stats = stats ?? new DashboardStatsDto(),
            RecentPOs = pos,
            Deliveries = deliveries
        };
    }
}