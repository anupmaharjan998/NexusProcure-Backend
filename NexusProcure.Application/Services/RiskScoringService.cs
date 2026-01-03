using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class RiskScoringService : IRiskScoringService
{
    private readonly NexusProcureDbContext _context;

    public RiskScoringService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<int> CalculateRiskScoreAsync(Requisition requisition)
    {
        int score = 0;

        // 1️⃣ Amount Risk
        var totalAmount = requisition.Items.Sum(i => i.EstimatedCost);
        if (totalAmount >= 500_000) score += 40;
        else if (totalAmount >= 100_000) score += 25;
        else if (totalAmount >= 50_000) score += 10;

        // 2️⃣ Category Risk
        var categoryRisk = await _context.Categories
            .Where(c => c.Id == requisition.CategoryId)
            .Select(c => c.RiskWeight)
            .FirstOrDefaultAsync();
        score += categoryRisk;

        // 3️⃣ Vendor Risk (Optional Phase-3.1)
        // if (requisition.VendorId != null)
        // {
        //     var vendorIsNew = !await _context.PurchaseOrders
        //         .AnyAsync(po => po.VendorId == requisition.VendorId);
        //     if (vendorIsNew) score += 10;
        // }

        // 4️⃣ User Risk
        var rejectionCount = await _context.Requisitions
            .CountAsync(r =>
                r.RequestedById == requisition.RequestedById &&
                r.Status == "Rejected");
        if (rejectionCount >= 3) score += 10;

        return Math.Min(score, 100);
    }

    public string ResolveRiskLevel(int score)
    {
        if (score >= 70) return "High";
        if (score >= 40) return "Medium";
        return "Low";
    }

    public async Task<RiskLevel> CalculateRiskLevelAsync(Requisition requisition)
    {
        int score = 0;

        // 1️⃣ Amount risk
        var totalAmount = requisition.Items.Sum(i => i.EstimatedCost);

        if (totalAmount >= 1_000_000) score += 40;
        else if (totalAmount >= 250_000) score += 25;
        else score += 10;

        // 2️⃣ Category risk (dynamic)
        var categoryRisk = await _context.Categories
            .Where(c => c.Id == requisition.CategoryId)
            .Select(c => c.RiskWeight)
            .FirstOrDefaultAsync();

        score += categoryRisk;

        // 3️⃣ Urgency risk
        if (requisition.IsUrgent)
            score += 15;

        // 4️⃣ Final mapping
        return score switch
        {
            >= 70 => RiskLevel.Critical,
            >= 50 => RiskLevel.High,
            >= 30 => RiskLevel.Medium,
            _ => RiskLevel.Low
        };
    }
    
    public async Task<RiskLevel> CalculateAsync(Guid requisitionId)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == requisitionId);

        if (requisition == null)
            throw new KeyNotFoundException("Requisition not found");

        return await CalculateRiskLevelAsync(requisition);
    }

}
