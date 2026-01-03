using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Approval;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class ApprovalPolicyService : IApprovalPolicyService
{
    private readonly NexusProcureDbContext _context;
    private readonly IRiskScoringService _riskScoringService;

    public ApprovalPolicyService(NexusProcureDbContext context, IRiskScoringService riskScoringService)
    {
        _context = context;
        _riskScoringService = riskScoringService;
    }

    public async Task<List<ApprovalLevel>> ResolveApprovalFlowAsync(Guid requisitionId)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == requisitionId);

        if (requisition == null)
            throw new KeyNotFoundException("Requisition not found");

        var riskLevel = await _riskScoringService.CalculateAsync(requisitionId);

        return await _context.ApprovalPolicies
            .Include(p => p.ApprovalLevel)
            .ThenInclude(l => l.Role)
            .Where(p =>
                p.CategoryId == requisition.CategoryId &&
                p.RiskLevel == riskLevel &&
                p.IsActive
            )
            .OrderBy(p => p.SequenceOrder)
            .Select(p => p.ApprovalLevel)
            .ToListAsync();
    }



    public async Task CreateAsync(ApprovalPolicyCreateDto dto)
    {
        var policy = new ApprovalPolicy
        {
            Id = Guid.NewGuid(),
            CategoryId = dto.CategoryId,
            ApprovalLevelId = dto.ApprovalLevelId,
            RiskLevel = dto.RiskLevel,
            SequenceOrder = dto.SequenceOrder,
            IsActive = true
        };

        _context.ApprovalPolicies.Add(policy);
        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(Guid id)
    {
        var policy = await _context.ApprovalPolicies.FindAsync(id);
        if (policy == null)
            throw new KeyNotFoundException("Policy not found");

        _context.ApprovalPolicies.Remove(policy);
        await _context.SaveChangesAsync();
    }
    

    public async Task<List<ApprovalPolicyResponseDto>> GetPoliciesAsync()
    {
        return await _context.ApprovalPolicies
            .Include(p => p.Category)
            .Include(p => p.ApprovalLevel)
            .ThenInclude(l => l.Role)
            .Select(p => new ApprovalPolicyResponseDto
            {
                Id = p.Id,
                CategoryName = p.Category.Name,
                RoleName = p.ApprovalLevel.Role.Name,
                RiskLevel = p.RiskLevel,
                SequenceOrder = p.SequenceOrder
            })
            .ToListAsync();
    }

}
