using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Approval;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;
using ApprovalDelegation = NexusProcure.Application.Interfaces.ApprovalDelegation;

namespace NexusProcure.Application.Services;

public class DelegationService : IDelegationService
{
    private readonly NexusProcureDbContext _context;

    public DelegationService(NexusProcureDbContext context)
    {
        _context = context;
    }
    
    public async Task<DelegationResponseDto> CreateAsync(CreateDelegationDto dto)
    {
        if (dto.StartDate >= dto.EndDate)
            throw new InvalidOperationException("End date must be after start date.");

        if (dto.FromUserId == dto.ToUserId)
            throw new InvalidOperationException("Cannot delegate to the same user.");

        // Prevent overlapping delegations
        var overlapExists = await _context.ApprovalDelegations.AnyAsync(d =>
            d.FromUserId == dto.FromUserId &&
            d.IsActive &&
            d.EndDate >= dto.StartDate &&
            d.StartDate <= dto.EndDate);

        if (overlapExists)
            throw new InvalidOperationException("Overlapping delegation already exists.");

        var delegation = new ApprovalDelegation
        {
            Id = Guid.NewGuid(),
            FromUserId = dto.FromUserId,
            ToUserId = dto.ToUserId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate
        };

        _context.ApprovalDelegations.Add(delegation);
        await _context.SaveChangesAsync();

        return new DelegationResponseDto
        {
            Id = delegation.Id,
            FromUserId = delegation.FromUserId,
            ToUserId = delegation.ToUserId,
            StartDate = delegation.StartDate,
            EndDate = delegation.EndDate,
            IsActive = delegation.IsActive
        };
    }

    public async Task<List<DelegationResponseDto>> GetActiveDelegationsAsync()
    {
        var now = DateTime.UtcNow;

        return await _context.ApprovalDelegations
            .Where(d =>
                d.IsActive &&
                d.StartDate <= now &&
                d.EndDate >= now)
            .Select(d => new DelegationResponseDto
            {
                Id = d.Id,
                FromUserId = d.FromUserId,
                ToUserId = d.ToUserId,
                StartDate = d.StartDate,
                EndDate = d.EndDate,
                IsActive = d.IsActive
            })
            .ToListAsync();
    }

    public async Task DeactivateAsync(Guid delegationId)
    {
        var delegation = await _context.ApprovalDelegations
            .FirstOrDefaultAsync(d => d.Id == delegationId);

        if (delegation == null)
            throw new KeyNotFoundException("Delegation not found.");

        delegation.IsActive = false;
        await _context.SaveChangesAsync();
    }

    public async Task<ApprovalDelegation?> GetValidDelegationAsync(
        Guid fromUserId,
        Guid toUserId,
        Guid? categoryId,
        Guid? approvalLevelId)
    {
        var now = DateTime.UtcNow;

        return await _context.ApprovalDelegations
            .Where(d =>
                d.FromUserId == fromUserId &&
                d.ToUserId == toUserId &&
                d.IsActive &&
                d.StartDate <= now &&
                d.EndDate >= now)
            .FirstOrDefaultAsync();
    }
}
