using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Reports;
using NexusProcure.Core.DTOs.Common;
using NexusProcure.Core.DTOs.Reports;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Reports;

public class ReportService : IReportService
{
    private readonly NexusProcureDbContext _context;

    public ReportService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<ReportsDashboardDto> GetDashboardAsync()
    {
        var currentYear = DateTime.UtcNow.Year;

        var poReport = await BuildPurchaseOrderReportListAsync();

        return new ReportsDashboardDto
        {
            Summary = await GetDashboardSummaryAsync(),
            RequisitionStatus = await GetRequisitionStatusAsync(),
            MonthlySpend = await GetMonthlySpendAsync(currentYear),
            LowStockItems = await GetLowStockReportAsync(),

            TodayDeliveries = poReport
                .Where(x => x.IsTodayDelivery)
                .OrderByDescending(x => x.TotalAmount)
                .Take(5)
                .ToList(),

            OverdueDeliveries = poReport
                .Where(x => x.IsOverdue)
                .OrderBy(x => x.ExpectedDeliveryDate)
                .Take(5)
                .ToList()
        };
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
{
    var purchaseOrders = await _context.PurchaseOrders
        .Include(x => x.Items)
        .ToListAsync();

    var poItemIds = purchaseOrders
        .SelectMany(x => x.Items)
        .Select(x => x.Id)
        .ToList();

    var receivedLookup = await _context.GoodsReceiptItems
        .Where(x => poItemIds.Contains(x.PurchaseOrderItemId))
        .GroupBy(x => x.PurchaseOrderItemId)
        .Select(g => new
        {
            PurchaseOrderItemId = g.Key,
            ReceivedQty = g.Sum(x => x.QuantityReceived)
        })
        .ToDictionaryAsync(x => x.PurchaseOrderItemId, x => x.ReceivedQty);

    var pendingDeliveries = purchaseOrders.Count(po =>
    {
        var ordered = po.Items.Sum(i => i.Quantity);

        var received = po.Items.Sum(i =>
            receivedLookup.TryGetValue(i.Id, out var receivedQty)
                ? receivedQty
                : 0
        );

        return received < ordered;
    });

    var completedDeliveries = purchaseOrders.Count(po =>
    {
        var ordered = po.Items.Sum(i => i.Quantity);

        var received = po.Items.Sum(i =>
            receivedLookup.TryGetValue(i.Id, out var receivedQty)
                ? receivedQty
                : 0
        );

        return ordered > 0 && received >= ordered;
    });

    var requisitionStatusCounts = await _context.Requisitions
        .GroupBy(x => x.Status)
        .Select(g => new
        {
            Status = g.Key,
            Count = g.Count()
        })
        .ToListAsync();

    var pendingApprovals = requisitionStatusCounts
        .Where(x => x.Status.ToString() == "PendingApproval")
        .Sum(x => x.Count);

    var approvedRequisitions = requisitionStatusCounts
        .Where(x => x.Status.ToString() == "Approved")
        .Sum(x => x.Count);

    var rejectedRequisitions = requisitionStatusCounts
        .Where(x => x.Status.ToString() == "Rejected")
        .Sum(x => x.Count);

    return new DashboardSummaryDto
    {
        TotalRequisitions = await _context.Requisitions.CountAsync(),

        PendingApprovals = pendingApprovals,

        ApprovedRequisitions = approvedRequisitions,

        RejectedRequisitions = rejectedRequisitions,

        TotalRfqs = await _context.RequestForQuotations.CountAsync(),

        TotalPurchaseOrders = purchaseOrders.Count,

        TotalPoValue = purchaseOrders
            .SelectMany(x => x.Items)
            .Sum(x => x.Quantity * x.UnitPrice),

        PendingDeliveries = pendingDeliveries,

        CompletedDeliveries = completedDeliveries,

        LowStockItems = await _context.InventoryStocks
            .CountAsync(x => x.QuantityAvailable <= x.ReorderLevel)
    };
}

   public async Task<List<ChartReportDto>> GetRequisitionStatusAsync()
   {
       var data = await _context.Requisitions
           .GroupBy(x => x.Status)
           .Select(g => new
           {
               Status = g.Key,
               Count = g.Count()
           })
           .OrderByDescending(x => x.Count)
           .ToListAsync();
   
       return data
           .Select(x => new ChartReportDto
           {
               Label = x.Status.ToString(),
               Count = x.Count
           })
           .ToList();
   }

    public async Task<List<MonthlySpendDto>> GetMonthlySpendAsync(int year)
    {
        var purchaseOrders = await _context.PurchaseOrders
            .Include(x => x.Items)
            .Where(x => x.CreatedAt.Year == year)
            .ToListAsync();

        return purchaseOrders
            .GroupBy(x => new
            {
                x.CreatedAt.Year,
                x.CreatedAt.Month
            })
            .Select(g => new MonthlySpendDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                TotalAmount = g.Sum(po => po.Items.Sum(i => i.Quantity * i.UnitPrice))
            })
            .OrderBy(x => x.Month)
            .ToList();
    }

    public async Task<PagedResult<PurchaseOrderReportDto>> GetPurchaseOrderReportAsync(
        PurchaseOrderReportQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

        var poReport = await BuildPurchaseOrderReportListAsync();

        if (query.FromDate.HasValue)
        {
            poReport = poReport
                .Where(x => x.CreatedAt.Date >= query.FromDate.Value.Date)
                .ToList();
        }

        if (query.ToDate.HasValue)
        {
            poReport = poReport
                .Where(x => x.CreatedAt.Date <= query.ToDate.Value.Date)
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            poReport = poReport
                .Where(x => x.Status == query.Status)
                .ToList();
        }

        var totalCount = poReport.Count;

        var items = poReport
            .OrderByDescending(x => x.IsTodayDelivery)
            .ThenByDescending(x => x.IsOverdue)
            .ThenByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedResult<PurchaseOrderReportDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<List<LowStockReportDto>> GetLowStockReportAsync()
    {
        return await _context.InventoryStocks
            .Include(x => x.Category)
            .Where(x => x.QuantityAvailable <= x.ReorderLevel)
            .OrderBy(x => x.QuantityAvailable)
            .Select(x => new LowStockReportDto
            {
                StockId = x.Id,
                ItemName = x.Name,
                CategoryName = x.Category.Name,
                Unit = x.Unit,
                AvailableQuantity = x.QuantityAvailable,
                ReorderLevel = x.ReorderLevel,
                StockStatus = x.QuantityAvailable <= 0
                    ? "Out of Stock"
                    : "Low Stock"
            })
            .ToListAsync();
    }


    private async Task<List<PurchaseOrderReportDto>> BuildPurchaseOrderReportListAsync()
    {
        var today = DateTime.UtcNow.Date;

        var purchaseOrders = await _context.PurchaseOrders
            .Include(x => x.Vendor)
            .Include(x => x.Requisition)
            .Include(x => x.Items)
            .ToListAsync();

        var poItemIds = purchaseOrders
            .SelectMany(x => x.Items)
            .Select(x => x.Id)
            .ToList();

        var receivedLookup = await _context.GoodsReceiptItems
            .Where(x => poItemIds.Contains(x.PurchaseOrderItemId))
            .GroupBy(x => x.PurchaseOrderItemId)
            .Select(g => new
            {
                PurchaseOrderItemId = g.Key,
                ReceivedQty = g.Sum(x => x.QuantityReceived)
            })
            .ToDictionaryAsync(x => x.PurchaseOrderItemId, x => x.ReceivedQty);

        var result = purchaseOrders.Select(x =>
        {
            var orderedQuantity = x.Items.Sum(i => i.Quantity);

            var receivedQuantity = x.Items.Sum(i =>
                receivedLookup.TryGetValue(i.Id, out var receivedQty)
                    ? receivedQty
                    : 0
            );

            var pendingQuantity = orderedQuantity - receivedQuantity;

            var totalAmount = x.Items.Sum(i => i.Quantity * i.UnitPrice);

            return new PurchaseOrderReportDto
            {
                Id = x.Id,

                PoNumber = x.PurchaseOrderNumber,

                VendorName = x.Vendor.CompanyName,

                RequisitionNumber = x.Requisition.RequisitionNumber,

                CreatedAt = x.CreatedAt,

                ExpectedDeliveryDate = x.DeliveryDate,

                Status = x.Status.ToString(),

                TotalAmount = totalAmount,

                OrderedQuantity = orderedQuantity,

                ReceivedQuantity = receivedQuantity,

                PendingQuantity = pendingQuantity,

                IsTodayDelivery =
                    x.DeliveryDate.HasValue
                    && x.DeliveryDate.Value.Date == today
                    && pendingQuantity > 0,

                IsOverdue =
                    x.DeliveryDate.HasValue
                    && x.DeliveryDate.Value.Date < today
                    && pendingQuantity > 0
            };
        }).ToList();

        return result;
    }
}