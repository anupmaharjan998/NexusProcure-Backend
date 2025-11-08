using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly NexusProcureDbContext _context;

    public DashboardService(NexusProcureDbContext context)
    {
        _context = context;
    }
    
    public async Task<DashboardStatsDto> GetStatsAsync()
    {
        var dto = new DashboardStatsDto
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalDepartments = await _context.Departments.CountAsync(),
            TotalRoles = await _context.Roles.CountAsync(),
            TotalPermissions = await _context.Permissions.CountAsync(),
            TotalVendors = await _context.Vendors.CountAsync(),
            TotalInventoryItems = await _context.InventoryItems.CountAsync(),
            TotalRequisitions = await _context.Requisitions.CountAsync(),
            TotalPurchaseOrders = await _context.PurchaseOrders.CountAsync()
        };

        return dto;
    }
}
