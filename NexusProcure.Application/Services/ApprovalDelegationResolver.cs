using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class ApprovalDelegationResolver : IApprovalDelegationResolver
{
    private readonly NexusProcureDbContext _context;

    public ApprovalDelegationResolver(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> ResolveApproverAsync(Guid originalApproverId)
    {
        var now = DateTime.UtcNow;

        var delegation = await _context.ApprovalDelegations
            .Where(d =>
                d.FromUserId == originalApproverId &&
                d.IsActive &&
                d.StartDate <= now &&
                d.EndDate >= now)
            .FirstOrDefaultAsync();

        return delegation?.ToUserId ?? originalApproverId;
    }
}
