using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Delegation;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class DelegationService : IDelegationService
{
    private readonly NexusProcureDbContext _context;

    private const string ManageDelegationPermission = "MANAGE_DELEGATION";
    private const string DelegationPermission = "DELEGATION";

    public DelegationService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<DelegationDto> CreateAsync(string currentUserClaim, CreateDelegationDto dto)
    {
        var currentUser = await GetCurrentUserAsync(currentUserClaim);

        var canManageAll = HasPermission(currentUser, ManageDelegationPermission);
        var canCreateOwn = HasPermission(currentUser, DelegationPermission);

        if (!canManageAll && !canCreateOwn)
        {
            throw new UnauthorizedAccessException("You do not have permission to create delegation.");
        }

        Guid delegatorUserId;

        if (canManageAll)
        {
            if (!dto.UserId.HasValue || dto.UserId.Value == Guid.Empty)
            {
                throw new InvalidOperationException("Delegator user is required.");
            }

            delegatorUserId = dto.UserId.Value;
        }
        else
        {
            if (dto.UserId.HasValue && dto.UserId.Value != currentUser.Id)
            {
                throw new UnauthorizedAccessException("You can only create delegation for yourself.");
            }

            delegatorUserId = currentUser.Id;
        }

        return await CreateDelegationInternalAsync(delegatorUserId, dto);
    }

    public async Task<List<DelegationDto>> GetVisibleDelegationsAsync(string currentUserClaim)
    {
        var currentUser = await GetCurrentUserAsync(currentUserClaim);

        var canManageAll = HasPermission(currentUser, ManageDelegationPermission);
        var canCreateOwn = HasPermission(currentUser, DelegationPermission);

        if (!canManageAll && !canCreateOwn)
        {
            throw new UnauthorizedAccessException("You do not have permission to view delegations.");
        }

        if (canManageAll)
        {
            return await GetAllAsync();
        }

        return await GetByUserAsync(currentUser.Id);
    }

    public async Task<List<DelegationDto>> GetMyDelegationsAsync(string currentUserClaim)
    {
        var currentUser = await GetCurrentUserAsync(currentUserClaim);

        var canManageAll = HasPermission(currentUser, ManageDelegationPermission);
        var canCreateOwn = HasPermission(currentUser, DelegationPermission);

        if (!canManageAll && !canCreateOwn)
        {
            throw new UnauthorizedAccessException("You do not have permission to view delegations.");
        }

        return await GetByUserAsync(currentUser.Id);
    }

    public async Task<DelegationPermissionsDto> GetPermissionsAsync(string currentUserClaim)
    {
        var currentUser = await GetCurrentUserAsync(currentUserClaim);

        var canManageAll = HasPermission(currentUser, ManageDelegationPermission);
        var canCreateOwn = HasPermission(currentUser, DelegationPermission);

        return new DelegationPermissionsDto
        {
            CanManageAll = canManageAll,
            CanCreateOwn = canManageAll || canCreateOwn
        };
    }

    public async Task<bool> DeactivateAsync(string currentUserClaim, Guid delegationId)
    {
        var currentUser = await GetCurrentUserAsync(currentUserClaim);

        var canManageAll = HasPermission(currentUser, ManageDelegationPermission);
        var canCreateOwn = HasPermission(currentUser, DelegationPermission);

        if (!canManageAll && !canCreateOwn)
        {
            throw new UnauthorizedAccessException("You do not have permission to revoke delegation.");
        }

        var delegation = await _context.UserDelegations
            .FirstOrDefaultAsync(x => x.Id == delegationId);

        if (delegation == null)
        {
            return false;
        }

        if (!canManageAll && delegation.UserId != currentUser.Id)
        {
            return false;
        }

        delegation.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<User?> GetActiveDelegateAsync(Guid userId)
    {
        var now = DateTime.UtcNow;

        var delegation = await _context.UserDelegations
            .Include(x => x.DelegateUser)
            .Where(x =>
                x.UserId == userId &&
                x.IsActive &&
                x.StartDate <= now &&
                x.EndDate >= now)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();

        return delegation?.DelegateUser;
    }

    public async Task ExpireDelegationsAsync()
    {
        var now = DateTime.UtcNow;

        var expiredDelegations = await _context.UserDelegations
            .Where(x => x.IsActive && x.EndDate < now)
            .ToListAsync();

        foreach (var delegation in expiredDelegations)
        {
            delegation.IsActive = false;
        }

        await _context.SaveChangesAsync();
    }

    private async Task<DelegationDto> CreateDelegationInternalAsync(Guid delegatorUserId, CreateDelegationDto dto)
    {
        if (delegatorUserId == Guid.Empty)
        {
            throw new InvalidOperationException("Delegator user is required.");
        }

        if (dto.DelegateUserId == Guid.Empty)
        {
            throw new InvalidOperationException("Delegate user is required.");
        }

        if (dto.DelegateUserId == delegatorUserId)
        {
            throw new InvalidOperationException("A user cannot delegate to themselves.");
        }

        var startDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
        var endDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);

        if (startDate >= endDate)
        {
            throw new InvalidOperationException("End date must be after start date.");
        }

        var delegatorExists = await _context.Users.AnyAsync(x => x.Id == delegatorUserId);
        if (!delegatorExists)
        {
            throw new InvalidOperationException("Delegator user not found.");
        }

        var delegateExists = await _context.Users.AnyAsync(x => x.Id == dto.DelegateUserId);
        if (!delegateExists)
        {
            throw new InvalidOperationException("Delegate user not found.");
        }

        var hasOverlappingActiveDelegation = await _context.UserDelegations.AnyAsync(x =>
            x.UserId == delegatorUserId &&
            x.IsActive &&
            x.EndDate >= startDate &&
            x.StartDate <= endDate);

        if (hasOverlappingActiveDelegation)
        {
            throw new InvalidOperationException(
                "An active delegation already exists for this user in the selected date range.");
        }

        var delegation = new UserDelegation
        {
            Id = Guid.NewGuid(),
            UserId = delegatorUserId,
            DelegateUserId = dto.DelegateUserId,
            StartDate = startDate,
            EndDate = endDate,
            Scope = string.IsNullOrWhiteSpace(dto.Scope) ? "All" : dto.Scope.Trim(),
            Reason = dto.Reason?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserDelegations.Add(delegation);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(delegation.Id);
    }

    private async Task<List<DelegationDto>> GetByUserAsync(Guid userId)
    {
        return await BaseDelegationQuery()
            .Where(x => x.UserId == userId || x.DelegateUserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => MapToDto(x))
            .ToListAsync();
    }

    private async Task<List<DelegationDto>> GetAllAsync()
    {
        return await BaseDelegationQuery()
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => MapToDto(x))
            .ToListAsync();
    }

    private async Task<DelegationDto> GetByIdAsync(Guid id)
    {
        var delegation = await BaseDelegationQuery()
            .Where(x => x.Id == id)
            .Select(x => MapToDto(x))
            .FirstOrDefaultAsync();

        if (delegation == null)
        {
            throw new InvalidOperationException("Delegation not found.");
        }

        return delegation;
    }

    private IQueryable<UserDelegation> BaseDelegationQuery()
    {
        return _context.UserDelegations
            .Include(x => x.User)
            .Include(x => x.DelegateUser)
            .AsNoTracking();
    }

    private static DelegationDto MapToDto(UserDelegation x)
    {
        return new DelegationDto
        {
            Id = x.Id,

            DelegatorUserId = x.UserId,
            DelegatorName = x.User.FullName,
            DelegatorEmail = x.User.Email,

            DelegateUserId = x.DelegateUserId,
            DelegateName = x.DelegateUser.FullName,
            DelegateEmail = x.DelegateUser.Email,

            StartDate = x.StartDate,
            EndDate = x.EndDate,
            Scope = x.Scope,
            Reason = x.Reason,

            IsActive = x.IsActive,
            IsExpired = x.EndDate < DateTime.UtcNow,

            Status = !x.IsActive
                ? "Revoked"
                : x.EndDate < DateTime.UtcNow
                    ? "Expired"
                    : x.StartDate > DateTime.UtcNow
                        ? "Scheduled"
                        : "Active",

            CreatedAt = x.CreatedAt
        };
    }

    private async Task<User> GetCurrentUserAsync(string claimValue)
    {
        if (string.IsNullOrWhiteSpace(claimValue))
        {
            throw new UnauthorizedAccessException("User claim was not found.");
        }

        User? user;

        if (Guid.TryParse(claimValue, out var userId))
        {
            user = await UserWithPermissionsQuery()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        else
        {
            var normalizedClaim = claimValue.Trim().ToLower();

            user = await UserWithPermissionsQuery()
                .FirstOrDefaultAsync(u =>
                    u.Email != null &&
                    u.Email.ToLower() == normalizedClaim);
        }

        if (user == null)
        {
            throw new UnauthorizedAccessException("Logged-in user was not found in database.");
        }

        return user;
    }

    private IQueryable<User> UserWithPermissionsQuery()
    {
        return _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission);
    }

    private static bool HasPermission(User user, string permissionKey)
    {
        return user.Role?.RolePermissions != null &&
               user.Role.RolePermissions.Any(rp =>
                   rp.Permission != null &&
                   rp.Permission.Key == permissionKey);
    }
}