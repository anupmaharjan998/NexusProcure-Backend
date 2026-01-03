using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.BackgroundJobs;

public class ApprovalEscalationJob : IApprovalEscalationJob
{
    private readonly NexusProcureDbContext _context;
    private readonly IEmailJobService _emailJobService;

    public ApprovalEscalationJob(NexusProcureDbContext context, IEmailJobService emailJobService)
    {
        _context = context;
        _emailJobService = emailJobService;
    }

    public async Task RunAsync()
    {
        var now = DateTime.UtcNow;

        var pendingApprovals = await _context.Approvals
            .Include(a => a.Requisition)
            .Include(a => a.ApprovalLevel)
            .Where(a =>
                a.Status == "Pending" &&
                !a.Escalated)
            .ToListAsync();

        foreach (var approval in pendingApprovals)
        {
            var policy = await _context.ApprovalPolicies
                .FirstOrDefaultAsync(p =>
                    p.ApprovalLevelId == approval.ApprovalLevelId &&
                    p.CategoryId == approval.Requisition.CategoryId &&
                    p.IsActive);

            if (policy == null)
                continue;

            var elapsedHours = (now - approval.AssignedAt).TotalHours;

            if (elapsedHours < policy.EscalationHours)
                continue;

            // 🔹 Find next approval policy in sequence
            var nextPolicy = await _context.ApprovalPolicies
                .Where(p =>
                    p.CategoryId == policy.CategoryId &&
                    p.RiskLevel == policy.RiskLevel &&
                    p.SequenceOrder > policy.SequenceOrder &&
                    p.IsActive)
                .OrderBy(p => p.SequenceOrder)
                .FirstOrDefaultAsync();

            if (nextPolicy == null)
                continue;

            // 🔹 Mark current approval as escalated
            approval.Escalated = true;
            approval.EscalatedAt = now;

            // 🔹 Assign new approval task
            var nextApproverUserId = await _context.Users
                .Where(u => u.RoleId == nextPolicy.ApprovalLevel.RoleId)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (nextApproverUserId == Guid.Empty)
                continue;

            var escalatedApproval = new Approval
            {
                Id = Guid.NewGuid(),
                RequisitionId = approval.RequisitionId,
                ApprovalLevelId = nextPolicy.ApprovalLevelId,
                AssignedToUserId = nextApproverUserId,
                AssignedAt = now,
                Status = "Pending"
            };

            _context.Approvals.Add(escalatedApproval);
            await _emailJobService.SendEscalationNotificationAsync(escalatedApproval.Id);
        }

        await _context.SaveChangesAsync();
        

    }
}
