using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Models;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class RiskScoringService : IRiskScoringService
{
    private readonly NexusProcureDbContext _context;
    private readonly IOptions<RiskSettings> _riskSettings;

    public RiskScoringService(
        NexusProcureDbContext context,
        IOptions<RiskSettings> riskSettings)
    {
        _context = context;
        _riskSettings = riskSettings;
    }

    public async Task<int> CalculateRiskScoreAsync(Requisition requisition)
    {
        if (requisition == null)
        {
            throw new KeyNotFoundException("Requisition not found");
        }

        if (requisition.Items == null || !requisition.Items.Any())
        {
            return 0;
        }

        var score = 0;

        // 1. Amount Risk
        // Uses total requisition amount: quantity * estimated cost.
        var totalAmount = requisition.Items.Sum(i => i.Quantity * i.EstimatedCost);

        var amountRule = await _context.TotalAmountRiskScores
            .Where(r =>
                r.IsActive &&
                totalAmount >= r.MinAmount &&
                (
                    r.MaxAmount == null ||
                    totalAmount < r.MaxAmount
                ))
            .OrderByDescending(r => r.MinAmount)
            .FirstOrDefaultAsync();

        if (amountRule != null)
        {
            score += amountRule.RiskPoints;
        }

        // 2. Category Risk
        // Category comes from InventoryStock.
        var categoryRisk = await GetCategoryRiskWeightAsync(requisition);
        score += categoryRisk;

        // 3. Urgency Risk
        if (requisition.IsUrgent)
        {
            score += _riskSettings.Value.UrgentRiskScore;
        }

        // 4. User Risk
        var rejectionCount = await _context.Requisitions
            .CountAsync(r =>
                r.RequestedById == requisition.RequestedById &&
                r.Status == "Rejected");

        if (rejectionCount >= 3)
        {
            score += 10;
        }

        return Math.Min(score, 100);
    }

    public RiskLevel ResolveRiskLevel(int score)
    {
        if (score >= _riskSettings.Value.CriticalScore)
            return RiskLevel.Critical;

        if (score >= _riskSettings.Value.HighScore)
            return RiskLevel.High;

        if (score >= _riskSettings.Value.MediumScore)
            return RiskLevel.Medium;

        return RiskLevel.Low;
    }

    public async Task<RiskLevel> CalculateRiskLevelAsync(Requisition requisition)
    {
        var score = await CalculateRiskScoreAsync(requisition);

        return ResolveRiskLevel(score);
    }

    private async Task<int> GetCategoryRiskWeightAsync(Requisition requisition)
    {
        var categoryRiskWeightsFromNavigation = requisition.Items
            .Where(i => i.InventoryStock?.Category != null)
            .Select(i => i.InventoryStock.Category.RiskWeight)
            .ToList();

        if (categoryRiskWeightsFromNavigation.Any())
        {
            // Use highest category risk among selected stock items.
            return categoryRiskWeightsFromNavigation.Max();
        }

        var stockIds = requisition.Items
            .Where(i => i.InventoryStockId != Guid.Empty)
            .Select(i => i.InventoryStockId)
            .Distinct()
            .ToList();

        if (!stockIds.Any())
        {
            return 0;
        }

        var categoryRiskWeights = await _context.InventoryStocks
            .Include(s => s.Category)
            .Where(s => stockIds.Contains(s.Id))
            .Select(s => s.Category.RiskWeight)
            .ToListAsync();

        if (!categoryRiskWeights.Any())
        {
            return 0;
        }

        // Best default for mixed-category requisition:
        // use the highest category risk instead of summing all categories.
        return categoryRiskWeights.Max();
    }
}