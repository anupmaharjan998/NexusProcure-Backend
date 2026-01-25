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

        public async Task ApproveAsync(
            Guid requisitionId,
            Guid approverId,
            string decision,
            string comments)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            bool sendStatusEmail = false;
            bool sendNextStepEmail = false;
            bool createRfq = false;

            try
            {
                var user = await _context.Users
                    .SingleAsync(u => u.Id == approverId);

                var approval = await _context.Approvals
                    .Include(a => a.Requisition)
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
                    sendStatusEmail = true;
                }
                else
                {
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

                            sendNextStepEmail = true;
                        }
                        else
                        {
                            approval.Requisition.Status = "Approved";
                            sendStatusEmail = true;
                            createRfq = true;
                        }
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

            // -----------------------------------
            // SIDE EFFECTS AFTER SUCCESSFUL COMMIT
            // -----------------------------------

            if (sendStatusEmail)
            {
                BackgroundJob.Enqueue<IEmailJobService>(job => job.SendApprovalStatusEmailAsync(requisitionId));
            }

            if (sendNextStepEmail)
            {
                BackgroundJob.Enqueue<IEmailJobService>(job => job.SendApprovalNotificationAsync(requisitionId));
            }

            if (createRfq)
            {
                BackgroundJob.Enqueue<IRfqJob>(job => job.CreateAndSendRfqAsync(requisitionId));
            }
        }
    }
}