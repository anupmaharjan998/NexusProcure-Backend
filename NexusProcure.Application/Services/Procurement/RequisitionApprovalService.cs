using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Procurement
{
    public class RequisitionApprovalService : IRequisitionApprovalService
    {
        private readonly NexusProcureDbContext _context;
        private readonly IMapper _mapper;
        private readonly IApprovalPolicyService _approvalPolicyService;

        public RequisitionApprovalService(NexusProcureDbContext context, IMapper mapper,
            IApprovalPolicyService approvalPolicyService)
        {
            _context = context;
            _mapper = mapper;
            _approvalPolicyService = approvalPolicyService;
        }

        public async Task ApproveRequisitionAsync(
            Guid requisitionId,
            Guid approverId,
            string decision,
            string comments)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var requisition = await _context.Requisitions
                    .Include(r => r.Items)
                    .Include(r => r.Approvals)
                    .FirstOrDefaultAsync(r => r.Id == requisitionId);

                if (requisition == null)
                    throw new KeyNotFoundException("Requisition not found");

                // 🔹 Get pending approval assigned to this user
                var approval = await _context.Approvals
                    .Include(a => a.Role)
                    .Where(a =>
                        a.RequisitionId == requisitionId &&
                        a.Status == "Pending" &&
                        //(a.AssignedToUserId == approverId ||
                        (_context.ApprovalDelegations.Any(d =>
                             //d.FromUserId == a.AssignedToUserId &&
                             d.ToUserId == approverId &&
                             d.IsActive)))
                    .FirstOrDefaultAsync();


                if (approval == null)
                    throw new UnauthorizedAccessException("No pending approval assigned");

                // 🔹 Update approval
                approval.Status = decision;
                approval.ApprovedById = approverId;
                approval.ActionedAt = DateTime.UtcNow;
                approval.Comments = comments;

                if (decision == "Rejected")
                {
                    requisition.Status = "Rejected";
                    // Queue email with Hangfire
                    BackgroundJob.Enqueue<IEmailJobService>(job => job.SendApprovalStatusEmailAsync(requisition.Id));
                }
                else
                {
                    // 🔹 Resolve full approval flow
                    var approvalFlow =
                        await _approvalPolicyService.ResolveApprovalFlowByIdAsync(requisitionId);

                    var nextLevel = approvalFlow
                        .SkipWhile(l => l.RoleId != approval.RoleId)
                        .Skip(1)
                        .FirstOrDefault();

                    if (nextLevel == null)
                    {
                        requisition.Status = "Approved";
                        BackgroundJob.Enqueue<IEmailJobService>(job => job.SendApprovalStatusEmailAsync(requisition.Id));
                    }
                    else
                    {
                        requisition.Status = "Partial Approved";

                        var nextApproverId = await _context.Users
                            .Where(u => u.RoleId == nextLevel.Id)
                            .Select(u => u.Id)
                            .FirstOrDefaultAsync();

                        if (nextApproverId == Guid.Empty)
                            throw new InvalidOperationException("Next approver not found");

                        var nextApproval = new Approval
                        {
                            Id = Guid.NewGuid(),
                            RequisitionId = requisitionId,
                            RoleId = nextLevel.Id,
                            //AssignedToUserId = nextApproverId,
                            AssignedAt = DateTime.UtcNow,
                            Status = "Pending"
                        };

                        await _context.Approvals.AddAsync(nextApproval);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


        [Obsolete("Replaced by ApprovalPolicyService")]
        public async Task<List<ApprovalLevelResponseDto>> GetRequiredLevelsAsync(decimal amount)
        {
            var approvals = await _context.ApprovalLevels
                // .Where(l => amount >= l.MinAmount)
                // .OrderBy(l => l.MinAmount) // ensures hierarchy
                .ToListAsync();

            return _mapper.Map<List<ApprovalLevelResponseDto>>(approvals);
        }

        public async Task<List<ApprovalDto>> GetApprovalsForRequisitionAsync(Guid requisitionId)
        {
            var requisition = await _context.Requisitions
                .Include(r => r.Approvals)
                .ThenInclude(a => a.ApprovedBy)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(r => r.Id == requisitionId);

            return _mapper.Map<List<ApprovalDto>>(requisition.Approvals.ToList());

            //return requisition?.Approvals.OrderBy(a => a.ApprovedDate).ToList() ?? new List<Approval>();
        }

        public async Task<List<RequisitionResponseDto>> GetPendingApprovalsForRoleAsync(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            var approvals = await _context.Approvals
                .Include(a => a.Requisition)
                .ThenInclude(r => r.Items)
                .Where(a =>
                    a.Status == "Pending" && a.IsActive && a.RoleId == user.RoleId)
                .Select(a => a.Requisition)
                .Distinct()
                .ToListAsync();


            return _mapper.Map<List<RequisitionResponseDto>>(approvals);
        }
        
        public async Task ApproveAsync(Guid requisitionId, Guid approverId, string decision, string comments)
        {
            var user =  _context.Users.SingleOrDefault(x => x.Id == approverId);
            var approval = await _context.Approvals
                    .Include(x => x.Requisition)
                .FirstAsync(a =>
                    a.RequisitionId == requisitionId &&
                    a.RoleId == user.RoleId &&
                    a.Status == "Pending");

            approval.ApprovedById = user.Id;
            approval.Status = decision;
            approval.Comments = comments;
            approval.ActionedAt = DateTime.UtcNow;

            if (decision == "Rejected")
            {
                approval.Requisition.Status = "Rejected";
                BackgroundJob.Enqueue<IEmailJobService>(job => job.SendApprovalStatusEmailAsync(requisitionId));
            }
            else
            {
                // Check if all approvals in this step are approved
                bool stepCompleted = !await _context.Approvals.AnyAsync(a =>
                    a.RequisitionId == requisitionId &&
                    a.SequenceOrder == approval.SequenceOrder &&
                    a.Status == "Pending");

                if (!stepCompleted)
                {
                    int nextStep = approval.SequenceOrder + 1;

                    var nextApprovals = await _context.Approvals
                        .Where(a =>
                            a.RequisitionId == requisitionId &&
                            a.SequenceOrder == nextStep)
                        .ToListAsync();

                    if (nextApprovals.Any())
                    {
                        foreach (var next in nextApprovals)
                        {
                            next.IsActive = true; 
                        }
                        BackgroundJob.Enqueue<IEmailJobService>(job => job.SendApprovalNotificationAsync(requisitionId));
                    }
                    else
                    {
                        approval.Requisition.Status = "Approved"; 
                        BackgroundJob.Enqueue<IEmailJobService>(job => job.SendApprovalStatusEmailAsync(requisitionId));
                    }
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}