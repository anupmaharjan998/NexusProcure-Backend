using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Approval;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class ApprovalPolicyService : IApprovalPolicyService
{
    private readonly NexusProcureDbContext _context;

    public ApprovalPolicyService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<List<ApprovalLevel>> ResolveApprovalFlowAsync(
        Guid categoryId,
        decimal amount)
    {
        return await _context.ApprovalPolicies
            .Include(p => p.ApprovalLevel)
            .Where(p =>
                p.CategoryId == categoryId &&
                p.IsActive &&
                amount >= p.MinAmount &&
                amount <= p.MaxAmount
            )
            .OrderBy(p => p.SequenceOrder)
            .Select(p => p.ApprovalLevel)
            .Distinct()
            .ToListAsync();
    }

    public async Task CreateAsync(ApprovalPolicyCreateDto dto)
    {
        var policy = new ApprovalPolicy
        {
            Id = Guid.NewGuid(),
            CategoryId = dto.CategoryId,
            ApprovalLevelId = dto.ApprovalLevelId,
            MinAmount = dto.MinAmount,
            MaxAmount = dto.MaxAmount,
            SequenceOrder = dto.SequenceOrder
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
                MinAmount = p.MinAmount,
                MaxAmount = p.MaxAmount,
                SequenceOrder = p.SequenceOrder
            })
            .ToListAsync();
    }
}
