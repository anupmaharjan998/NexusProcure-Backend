using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Approval;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class ApprovalPolicyService : IApprovalPolicyService
{
    private readonly NexusProcureDbContext _context;
    private readonly IRiskScoringService _riskScoringService;

    public ApprovalPolicyService(
        NexusProcureDbContext context,
        IRiskScoringService riskScoringService)
    {
        _context = context;
        _riskScoringService = riskScoringService;
    }

    public async Task<List<ApprovalPolicy>> ResolveApprovalFlowByIdAsync(Guid requisitionId)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
                .ThenInclude(i => i.InventoryStock)
                    .ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(r => r.Id == requisitionId);

        if (requisition == null)
        {
            throw new KeyNotFoundException("Requisition not found");
        }

        return await ResolveApprovalFlowAsync(requisition);
    }

    public async Task<List<ApprovalPolicy>> ResolveApprovalFlowAsync(Requisition requisition)
    {
        if (requisition == null)
        {
            throw new KeyNotFoundException("Requisition not found");
        }

        var categoryIds = requisition.Items
            .Where(i => i.InventoryStock?.CategoryId != null)
            .Select(i => i.InventoryStock.CategoryId)
            .Distinct()
            .ToList();

        if (!categoryIds.Any())
        {
            var stockIds = requisition.Items
                .Where(i => i.InventoryStockId != Guid.Empty)
                .Select(i => i.InventoryStockId)
                .Distinct()
                .ToList();

            categoryIds = await _context.InventoryStocks
                .Where(s => stockIds.Contains(s.Id))
                .Select(s => s.CategoryId)
                .Distinct()
                .ToListAsync();
        }

        if (!categoryIds.Any())
        {
            throw new InvalidOperationException(
                "No inventory stock categories found for this requisition.");
        }

        var riskLevel = requisition.RiskLevel;

        var policies = await _context.ApprovalPolicies
            .Include(p => p.Role)
            .Include(p => p.Category)
            .Where(p =>
                categoryIds.Contains(p.CategoryId) &&
                p.RiskLevel == riskLevel &&
                p.IsActive)
            .OrderBy(p => p.SequenceOrder)
            .ThenBy(p => p.Role.Name)
            .ToListAsync();

        if (!policies.Any())
        {
            throw new InvalidOperationException(
                $"No active approval policy found for risk level {riskLevel} and selected inventory categories.");
        }

        return policies
            .GroupBy(p => new
            {
                p.SequenceOrder,
                p.RoleId
            })
            .Select(g => g.First())
            .OrderBy(p => p.SequenceOrder)
            .ThenBy(p => p.Role.Name)
            .ToList();
    }

    public async Task CreateAsync(ApprovalPolicyCreateDto dto)
    {
        var categoryExists = await _context.InventoryCategories
            .AnyAsync(c => c.Id == dto.CategoryId);

        if (!categoryExists)
        {
            throw new InvalidOperationException("Inventory category not found.");
        }

        var roleExists = await _context.Roles
            .AnyAsync(r => r.Id == dto.RoleId);

        if (!roleExists)
        {
            throw new InvalidOperationException("Role not found.");
        }

        if (!Enum.TryParse<RiskLevel>(dto.RiskLevel, true, out var riskLevel))
        {
            throw new InvalidOperationException("Invalid risk level.");
        }

        var duplicateExists = await _context.ApprovalPolicies.AnyAsync(p =>
            p.CategoryId == dto.CategoryId &&
            p.RoleId == dto.RoleId &&
            p.RiskLevel == riskLevel &&
            p.SequenceOrder == dto.SequenceOrder &&
            p.IsActive);

        if (duplicateExists)
        {
            throw new InvalidOperationException(
                "An active approval policy already exists for this category, role, risk level, and sequence.");
        }

        var policy = new ApprovalPolicy
        {
            Id = Guid.NewGuid(),
            CategoryId = dto.CategoryId,
            RoleId = dto.RoleId,
            RiskLevel = riskLevel,
            SequenceOrder = dto.SequenceOrder,
            EscalationHours = dto.EscalationHours,
            IsActive = true
        };

        _context.ApprovalPolicies.Add(policy);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var policy = await _context.ApprovalPolicies.FindAsync(id);

        if (policy == null)
        {
            throw new KeyNotFoundException("Policy not found");
        }

        _context.ApprovalPolicies.Remove(policy);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ApprovalPolicyResponseDto>> GetPoliciesAsync()
    {
        return await _context.ApprovalPolicies
            .Include(p => p.Category)
            .Include(p => p.Role)
            .OrderBy(p => p.Category.Name)
            .ThenBy(p => p.RiskLevel)
            .ThenBy(p => p.SequenceOrder)
            .Select(p => new ApprovalPolicyResponseDto
            {
                Id = p.Id,
                CategoryName = p.Category.Name,
                RoleName = p.Role.Name,
                RiskLevel = p.RiskLevel.ToString(),
                SequenceOrder = p.SequenceOrder,
                EscalationHours = p.EscalationHours,
                IsActive = p.IsActive
            })
            .ToListAsync();
    }
}