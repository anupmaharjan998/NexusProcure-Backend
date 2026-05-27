using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.BackgroundJobs;

public class ApprovalEscalationJob : IApprovalEscalationJob
{
    private readonly NexusProcureDbContext _context;
    private readonly IEmailJobService _emailJobService;

    public ApprovalEscalationJob(
        NexusProcureDbContext context,
        IEmailJobService emailJobService)
    {
        _context = context;
        _emailJobService = emailJobService;
    }

    public async Task RunAsync()
    {
        var now = DateTime.UtcNow;

        var pendingApprovals = await _context.Approvals
            .Include(a => a.Role)
            .Where(a =>
                a.Status == "Pending" &&
                a.IsActive &&
                !a.Escalated)
            .ToListAsync();

        foreach (var approval in pendingApprovals)
        {
            var categoryIds = await GetCategoryIdsAsync(
                approval.ReferenceId,
                approval.ReferenceType);

            if (!categoryIds.Any())
            {
                continue;
            }

            var policy = await _context.ApprovalPolicies
                .Where(p =>
                    p.RoleId == approval.RoleId &&
                    categoryIds.Contains(p.CategoryId) &&
                    p.IsActive)
                .OrderBy(p => p.EscalationHours)
                .FirstOrDefaultAsync();

            if (policy == null)
            {
                continue;
            }

            var elapsedHours = (now - approval.AssignedAt).TotalHours;

            if (elapsedHours < policy.EscalationHours)
            {
                continue;
            }

            await UpdateReferenceStatusAsync(
                approval.ReferenceId,
                approval.ReferenceType,
                "Rejected");

            approval.Status = "EscalationExpired";
            approval.Escalated = true;
            approval.EscalatedAt = now;
            approval.IsActive = false;
            approval.Comments = "Auto-rejected due to approval escalation timeout";
        }

        await _context.SaveChangesAsync();
    }

    private async Task<List<Guid>> GetCategoryIdsAsync(
        Guid referenceId,
        ApprovalReferenceType referenceType)
    {
        if (referenceType == ApprovalReferenceType.Requisition)
        {
            return await _context.RequisitionItems
                .Where(i => i.RequisitionId == referenceId)
                .Select(i => i.InventoryStock.CategoryId)
                .Distinct()
                .ToListAsync();
        }

        if (referenceType == ApprovalReferenceType.RFQ)
        {
            var requisitionId = await _context.RequestForQuotations
                .Where(r => r.Id == referenceId)
                .Select(r => r.RequisitionId)
                .FirstOrDefaultAsync();

            if (requisitionId == Guid.Empty)
            {
                return new List<Guid>();
            }

            return await _context.RequisitionItems
                .Where(i => i.RequisitionId == requisitionId)
                .Select(i => i.InventoryStock.CategoryId)
                .Distinct()
                .ToListAsync();
        }

        return new List<Guid>();
    }

    private async Task UpdateReferenceStatusAsync(
        Guid referenceId,
        ApprovalReferenceType referenceType,
        string status)
    {
        if (referenceType == ApprovalReferenceType.Requisition)
        {
            var requisition = await _context.Requisitions
                .FirstOrDefaultAsync(r => r.Id == referenceId);

            if (requisition != null)
            {
                requisition.Status = status;
            }

            return;
        }

        if (referenceType == ApprovalReferenceType.RFQ)
        {
            var rfq = await _context.RequestForQuotations
                .FirstOrDefaultAsync(r => r.Id == referenceId);

            if (rfq != null)
            {
                rfq.Status = status == "Rejected"
                    ? RfqStatus.Cancelled
                    : Enum.Parse<RfqStatus>(status);
            }
        }
    }
}