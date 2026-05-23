using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Delegation;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class DelegationService : IDelegationService
{
    private readonly NexusProcureDbContext _context;
    private readonly IAuditService _auditService;

    public DelegationService(NexusProcureDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<DelegationDto> CreateAsync(Guid userId, CreateDelegationDto dto)
    {
        if (dto.StartDate >= dto.EndDate)
            throw new Exception("Invalid date range");

        if (userId == dto.DelegateUserId)
            throw new Exception("Cannot delegate to self");

        // 🔹 Validate delegator
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new Exception("User not found");

        // 🔹 Fetch delegate user (FIXED)
        var delegateUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == dto.DelegateUserId);

        if (delegateUser == null)
            throw new Exception("Delegate user not found");

        if (!delegateUser.IsActive)
            throw new Exception("Cannot delegate to inactive user");

        // Prevent delegating to subordinate (direct)
        if (delegateUser.ManagerId == userId)
            throw new Exception("Cannot delegate to direct subordinate");

        // OPTIONAL: Prevent deep hierarchy delegation (recommended)
        if (await IsSubordinate(userId, delegateUser.Id))
            throw new Exception("Cannot delegate to subordinate in hierarchy");

        // 🔹 Proper overlap check
        var overlapping = await _context.UserDelegations
            .AnyAsync(d =>
                d.UserId == userId &&
                d.IsActive &&
                (
                    dto.StartDate <= d.EndDate &&
                    dto.EndDate >= d.StartDate
                ));

        if (overlapping)
            throw new Exception("Delegation overlaps with existing active delegation");

        var delegation = new UserDelegation
        {
            UserId = userId,
            DelegateUserId = dto.DelegateUserId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true,
            Reason = dto.Reason
        };

        _context.UserDelegations.Add(delegation);
        await _context.SaveChangesAsync();
        
        await _auditService.LogAsync(
            "UserDelegation",
            delegation.Id,
            "Created",
            userId,
            null,
            new
            {
                delegation.UserId,
                delegation.DelegateUserId,
                delegation.StartDate,
                delegation.EndDate,
                delegation.Reason
            });

        return new DelegationDto
        {
            Id = delegation.Id,
            UserId = userId,
            DelegateUserId = dto.DelegateUserId,
            UserName = user.FullName,
            DelegateUserName = delegateUser.FullName,
            StartDate = delegation.StartDate,
            EndDate = delegation.EndDate,
            IsActive = delegation.IsActive
        };
    }

    public async Task<bool> DeactivateAsync(Guid delegationId)
    {
        var delegation = await _context.UserDelegations.FindAsync(delegationId);
        if (delegation == null) return false;

        delegation.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<User?> GetActiveDelegateAsync(Guid userId)
    {
        var now = DateTime.UtcNow;

        var delegation = await _context.UserDelegations
            .Include(d => d.DelegateUser)
            .FirstOrDefaultAsync(d =>
                d.UserId == userId &&
                d.IsActive &&
                d.StartDate <= now &&
                d.EndDate >= now);

        if (delegation == null)
            return null;

        var oldValues = new
        {
            delegation.IsActive
        };

        delegation.IsActive = false;

        await _context.SaveChangesAsync();

        await _auditService.LogAsync(
            "UserDelegation",
            delegation.Id,
            "Deactivated",
            userId,
            oldValues,
            new
            {
                delegation.IsActive
            });

        return delegation.DelegateUser;
    }
    
    
    public async Task ExpireDelegationsAsync()
    {
        var now = DateTime.UtcNow;

        var expiredDelegations = await _context.UserDelegations
            .Where(d => d.IsActive && d.EndDate < now)
            .ToListAsync();

        if (!expiredDelegations.Any())
            return;

        foreach (var delegation in expiredDelegations)
        {
            var oldValues = new { delegation.IsActive };

            delegation.IsActive = false;

            await _auditService.LogAsync(
                "UserDelegation",
                delegation.Id,
                "Expired",
                null, // system action
                oldValues,
                new { delegation.IsActive });
        }

        await _context.SaveChangesAsync();
    }
    

    private async Task<bool> IsSubordinate(Guid managerId, Guid targetUserId)
    {
        var current = await _context.Users.FindAsync(targetUserId);

        while (current?.ManagerId != null)
        {
            if (current.ManagerId == managerId)
                return true;

            current = await _context.Users.FindAsync(current.ManagerId);
        }

        return false;
    }
}