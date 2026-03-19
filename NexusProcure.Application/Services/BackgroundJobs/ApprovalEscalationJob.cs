using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;
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

    // public async Task RunAsync()
    // {
    //     var now = DateTime.UtcNow;
    //
    //     var pendingApprovals = await _context.Approvals
    //         .Include(a => a.Requisition)
    //         .Include(a => a.Role)
    //         .Where(a =>
    //             a.Status == "Pending" && a.IsActive &&
    //             !a.Escalated)
    //         .ToListAsync();
    //
    //     foreach (var approval in pendingApprovals)
    //     {
    //         var policy = await _context.ApprovalPolicies
    //             .FirstOrDefaultAsync(p =>
    //                 p.RoleId == approval.RoleId &&
    //                 p.CategoryId == approval.Requisition.CategoryId &&
    //                 p.IsActive);
    //
    //         if (policy == null)
    //             continue;
    //
    //         var elapsedHours = (now - approval.AssignedAt).TotalHours;
    //
    //         if (elapsedHours < policy.EscalationHours)
    //             continue;
    //         
    //         approval.Requisition.Status = "Rejected";
    //         //approval.Requisition. = now;
    //         approval.Status = "EscalationExpired";
    //         approval.Escalated = true;
    //         approval.EscalatedAt = now;
    //         approval.Comments = "Auto-rejected due to approval escalation timeout";
    //
    //         // // 🔹 Find next approval policy in sequence
    //         // var nextPolicy = await _context.ApprovalPolicies
    //         //     .Where(p =>
    //         //         p.CategoryId == policy.CategoryId &&
    //         //         p.RiskLevel == policy.RiskLevel &&
    //         //         p.SequenceOrder > policy.SequenceOrder &&
    //         //         p.IsActive)
    //         //     .OrderBy(p => p.SequenceOrder)
    //         //     .FirstOrDefaultAsync();
    //         //
    //         // if (nextPolicy == null)
    //         //     continue;
    //         //
    //         // // 🔹 Mark current approval as escalated
    //         // approval.Escalated = true;
    //         // approval.EscalatedAt = now;
    //         //
    //         // // 🔹 Assign new approval task
    //         // var nextApproverUserId = await _context.Users
    //         //     .Where(u => u.RoleId == nextPolicy.RoleId)
    //         //     .Select(u => u.Id)
    //         //     .FirstOrDefaultAsync();
    //         //
    //         // if (nextApproverUserId == Guid.Empty)
    //         //     continue;
    //         //
    //         // var escalatedApproval = new Approval
    //         // {
    //         //     Id = Guid.NewGuid(),
    //         //     RequisitionId = approval.RequisitionId,
    //         //     RoleId = nextPolicy.RoleId,
    //         //     //AssignedToUserId = nextApproverUserId,
    //         //     AssignedAt = now,
    //         //     Status = "Pending"
    //         // };
    //         //
    //         // _context.Approvals.Add(escalatedApproval);
    //         //await _emailJobService.SendEscalationNotificationAsync(escalatedApproval.Id);
    //     }
    //
    //     await _context.SaveChangesAsync();
    //     
    //
    // }
    
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
            var categoryId = await GetCategoryIdAsync(
                approval.ReferenceId,
                approval.ReferenceType);

            if (categoryId == null)
                continue;

            var policy = await _context.ApprovalPolicies
                .FirstOrDefaultAsync(p =>
                    p.RoleId == approval.RoleId &&
                    p.CategoryId == categoryId &&
                    p.IsActive);

            if (policy == null)
                continue;

            var elapsedHours = (now - approval.AssignedAt).TotalHours;

            if (elapsedHours < policy.EscalationHours)
                continue;

            // ------------------------------------
            // Escalation Expired → Auto Reject
            // ------------------------------------

            await UpdateReferenceStatusAsync(
                approval.ReferenceId,
                approval.ReferenceType,
                "Rejected");

            approval.Status = "EscalationExpired";
            approval.Escalated = true;
            approval.EscalatedAt = now;
            approval.IsActive = false;
            approval.Comments =
                "Auto-rejected due to approval escalation timeout";
        }

        await _context.SaveChangesAsync();
    }
    private async Task<Guid?> GetCategoryIdAsync(
        Guid referenceId,
        ApprovalReferenceType referenceType)
    {
        if (referenceType == ApprovalReferenceType.Requisition)
        {
            return await _context.Requisitions
                .Where(r => r.Id == referenceId)
                .Select(r => r.CategoryId)
                .FirstOrDefaultAsync();
        }

        // if (referenceType == ApprovalReferenceType.RFQ)
        // {
        //     return await _context.RequestForQuotations
        //         .Include(x => x.Vendors)
        //         .ThenInclude(v => v.Vendor.Category)
        //         .Where(r => r.Id == referenceId)
        //         .Select(r => r.Vendors.Category)
        //         .FirstOrDefaultAsync();
        // }
        if (referenceType == ApprovalReferenceType.RFQ)
        {
            // Step 1: Get RequisitionId from RFQ
            var requisitionId = await _context.RequestForQuotations
                .Where(r => r.Id == referenceId)
                .Select(r => r.RequisitionId)
                .FirstOrDefaultAsync();

            if (requisitionId == Guid.Empty)
                return null;

            // Step 2: Get CategoryId from Requisition
            return await _context.Requisitions
                .Where(r => r.Id == requisitionId)
                .Select(r => r.CategoryId)
                .FirstOrDefaultAsync();
        }
        
        return null;
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
                requisition.Status = status;
        }

        if (referenceType == ApprovalReferenceType.RFQ)
        {
            var rfq = await _context.RequestForQuotations
                .FirstOrDefaultAsync(r => r.Id == referenceId);

            if (rfq != null)
                rfq.Status = Enum.Parse<RfqStatus>(status);
        }

        // if (referenceType == ApprovalReferenceType.PurchaseRequest)
        // {
        //     var pr = await _context.PurchaseRequests
        //         .FirstOrDefaultAsync(p => p.Id == referenceId);
        //
        //     if (pr != null)
        //         pr.Status = status;
        // }
    }

}
