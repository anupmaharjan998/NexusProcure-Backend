using AutoMapper;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.DTOs.RFQ;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Procurement
{
    public class RequisitionApprovalService : IRequisitionApprovalService
    {
        private readonly NexusProcureDbContext _context;
        private readonly IMapper _mapper;
        private readonly IApprovalPolicyService _approvalPolicyService;

        public RequisitionApprovalService(
            NexusProcureDbContext context,
            IMapper mapper,
            IApprovalPolicyService approvalPolicyService)
        {
            _context = context;
            _mapper = mapper;
            _approvalPolicyService = approvalPolicyService;
        }

        [Obsolete("Replaced by ApprovalPolicyService")]
        public async Task<List<ApprovalLevelResponseDto>> GetRequiredLevelsAsync(decimal amount)
        {
            var approvals = await _context.ApprovalLevels.ToListAsync();
            return _mapper.Map<List<ApprovalLevelResponseDto>>(approvals);
        }

        public async Task<List<ApprovalDto>> GetApprovalsForRequisitionAsync(Guid requisitionId)
        {
            var requisition = await _context.Requisitions
                .Include(r => r.Approvals)
                    .ThenInclude(a => a.ApprovedBy)
                    .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(r => r.Id == requisitionId);

            if (requisition == null)
                return new List<ApprovalDto>();

            return _mapper.Map<List<ApprovalDto>>(requisition.Approvals.ToList());
        }

        public async Task<List<RequisitionResponseDto>> GetPendingApprovalsForRoleAsync(Guid userId)
        {
            var user = await _context.Users
                .Include(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            var effectiveRoleIds = await GetEffectiveApprovalRoleIdsAsync(userId);

            var requisitionIds = await _context.Approvals
                .Where(a =>
                    a.Status == "Pending" &&
                    a.IsActive &&
                    a.ReferenceType == ApprovalReferenceType.Requisition &&
                    effectiveRoleIds.Contains(a.RoleId))
                .Select(a => a.ReferenceId)
                .Distinct()
                .ToListAsync();

            var requisitions = await _context.Requisitions
                .Include(r => r.Items)
                .Where(r => requisitionIds.Contains(r.Id))
                .ToListAsync();

            return _mapper.Map<List<RequisitionResponseDto>>(requisitions);
        }

        public async Task<List<QuotationApprovalListResponseDto>> GetPendingQuotationApprovalsForRoleAsync(Guid userId)
        {
            var userExists = await _context.Users
                .AnyAsync(x => x.Id == userId);

            if (!userExists)
                throw new Exception("User not found");

            var effectiveRoleIds = await GetEffectiveApprovalRoleIdsAsync(userId);

            // ReferenceId contains RequisitionId
            var pendingRequisitionIds = await _context.Approvals
                .Where(a =>
                    a.Status == "Pending" &&
                    a.IsActive &&
                    a.ReferenceType == ApprovalReferenceType.RFQ &&
                    effectiveRoleIds.Contains(a.RoleId))
                .Select(a => a.ReferenceId)
                .Distinct()
                .ToListAsync();

            if (!pendingRequisitionIds.Any())
                return new List<QuotationApprovalListResponseDto>();

            // Find RFQs created for those requisitions
            var pendingRfqIds = await _context.RequestForQuotations
                .Where(rfq => pendingRequisitionIds.Contains(rfq.RequisitionId))
                .Select(rfq => rfq.Id)
                .Distinct()
                .ToListAsync();

            if (!pendingRfqIds.Any())
                return new List<QuotationApprovalListResponseDto>();

            // Find selected quotations of those RFQs
            var quotations = await _context.Quotations
                .AsNoTracking()
                .Include(q => q.RequestForQuotation)
                .ThenInclude(rfq => rfq.Requisition)
                .Include(q => q.RfqVendor)
                .ThenInclude(rv => rv.Vendor)
                .Include(q => q.Items)
                .Where(q =>
                    pendingRfqIds.Contains(q.RfqId) &&
                    q.IsSelected)
                .OrderByDescending(q => q.SubmittedAt)
                .ToListAsync();

            return _mapper.Map<List<QuotationApprovalListResponseDto>>(quotations);
        }

        public Task ApproveQuoatationAsync(Guid quotationId, Guid approverId, string decision, string comments,
            ApprovalReferenceType referenceType)
        {
            var rfqId = _context.Quotations.Where(q => q.Id == quotationId).Select(q => q.RfqId).FirstOrDefault();
            var requisitionId = _context.RequestForQuotations.Where(r => r.Id == rfqId).Select(r => r.RequisitionId).FirstOrDefault();
            return ApproveAsync(requisitionId, approverId, decision, comments, referenceType);
        }

        public async Task ApproveAsync(
            Guid referenceId,
            Guid approverId,
            string decision,
            string comments,
            ApprovalReferenceType referenceType)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            bool sendStatusEmail = false;
            bool sendNextStepEmail = false;
            bool createRfq = false;
            bool createPurchaseRequest = false;

            try
            {
                var approver = await _context.Users
                    .Include(u => u.Role)
                    .SingleOrDefaultAsync(u => u.Id == approverId);

                if (approver == null)
                    throw new Exception("Approver not found");

                var effectiveRoleIds = await GetEffectiveApprovalRoleIdsAsync(approverId);

                if (referenceType == ApprovalReferenceType.RFQ)
                {
                    var rfqId = _context.Quotations.Where(q => q.Id == referenceId).Select(q => q.RfqId).FirstOrDefault();
                    referenceId = _context.RequestForQuotations.Where(r => r.Id == rfqId).Select(r => r.RequisitionId).FirstOrDefault();
                }
                var approval = await _context.Approvals
                    .FirstOrDefaultAsync(a =>
                        a.ReferenceId == referenceId &&
                        a.ReferenceType == referenceType &&
                        a.Status == "Pending" &&
                        a.IsActive &&
                        effectiveRoleIds.Contains(a.RoleId));

                if (approval == null)
                {
                    throw new UnauthorizedAccessException(
                        "You are not allowed to approve this request. No matching role or active delegation was found.");
                }

                var delegatedFromUser = await GetDelegatedFromUserForRoleAsync(
                    delegateUserId: approverId,
                    approvalRoleId: approval.RoleId);

                var isDelegatedApproval =
                    delegatedFromUser != null &&
                    delegatedFromUser.Id != approverId;

                approval.ApprovedById = approver.Id;
                approval.Status = decision;
                approval.Comments = BuildApprovalComment(
                    comments,
                    isDelegatedApproval,
                    delegatedFromUser,
                    approver);
                approval.ActionedAt = DateTime.UtcNow;
                approval.IsActive = false;

                if (decision == "Rejected")
                {
                    await MarkReferenceRejectedAsync(referenceId, referenceType);
                    sendStatusEmail = true;
                }
                else if (decision == "Approved")
                {
                    var currentStepCompleted = !await _context.Approvals.AnyAsync(a =>
                        a.ReferenceId == referenceId &&
                        a.ReferenceType == referenceType &&
                        a.SequenceOrder == approval.SequenceOrder &&
                        a.Status == "Pending");

                    if (!currentStepCompleted)
                    {
                        var nextStep = approval.SequenceOrder + 1;

                        var nextApprovals = await _context.Approvals
                            .Where(a =>
                                a.ReferenceId == referenceId &&
                                a.ReferenceType == referenceType &&
                                a.SequenceOrder == nextStep &&
                                a.Status == "Pending")
                            .ToListAsync();

                        if (nextApprovals.Any())
                        {
                            foreach (var nextApproval in nextApprovals)
                            {
                                nextApproval.IsActive = true;
                            }

                            sendNextStepEmail = true;
                        }
                        else
                        {
                            await MarkReferenceApprovedAsync(referenceId, referenceType);

                            sendStatusEmail = true;

                            if (referenceType == ApprovalReferenceType.Requisition)
                            {
                                createRfq = true;
                            }

                            if (referenceType == ApprovalReferenceType.RFQ)
                            {
                                createPurchaseRequest = true;
                            }
                        }
                    }
                }
                else
                {
                    throw new InvalidOperationException("Invalid decision. Use Approved or Rejected.");
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            if (sendStatusEmail && referenceType == ApprovalReferenceType.Requisition)
            {
                BackgroundJob.Enqueue<IEmailJobService>(
                    job => job.SendApprovalStatusEmailAsync(referenceId));
            }

            if (sendNextStepEmail && referenceType == ApprovalReferenceType.Requisition)
            {
                BackgroundJob.Enqueue<IEmailJobService>(
                    job => job.SendApprovalNotificationAsync(referenceId));
            }

            if (createRfq)
            {
                BackgroundJob.Enqueue<IRfqJob>(
                    job => job.CreateAndSendRfqAsync(referenceId));
            }

            // Add your RFQ/Purchase Request background job here if you already have one.
            // Example:
            if (createPurchaseRequest)
            {
                BackgroundJob.Enqueue<IPurchaseRequestJob>(
                    job => job.CreatePurchaseRequestAsync(referenceId));
            }
        }

        private async Task<List<Guid>> GetEffectiveApprovalRoleIdsAsync(Guid userId)
        {
            var now = DateTime.UtcNow;

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            var roleIds = new List<Guid>
            {
                user.RoleId
            };

            var delegatedRoleIds = await _context.UserDelegations
                .Include(d => d.User)
                .Where(d =>
                    d.DelegateUserId == userId &&
                    d.IsActive &&
                    d.StartDate <= now &&
                    d.EndDate >= now)
                .Select(d => d.User.RoleId)
                .Distinct()
                .ToListAsync();

            roleIds.AddRange(delegatedRoleIds);

            return roleIds.Distinct().ToList();
        }

        private async Task<User?> GetDelegatedFromUserForRoleAsync(
            Guid delegateUserId,
            Guid approvalRoleId)
        {
            var now = DateTime.UtcNow;

            return await _context.UserDelegations
                .Include(d => d.User)
                .Where(d =>
                    d.DelegateUserId == delegateUserId &&
                    d.User.RoleId == approvalRoleId &&
                    d.IsActive &&
                    d.StartDate <= now &&
                    d.EndDate >= now)
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => d.User)
                .FirstOrDefaultAsync();
        }

        private string BuildApprovalComment(
            string comments,
            bool isDelegatedApproval,
            User? delegatedFromUser,
            User approver)
        {
            var cleanComments = comments?.Trim() ?? string.Empty;

            if (!isDelegatedApproval || delegatedFromUser == null)
                return cleanComments;

            var delegationNote =
                $"[Delegated Approval] Original approver role user: {delegatedFromUser.FullName}. " +
                $"Action performed by delegate: {approver.FullName}.";

            if (string.IsNullOrWhiteSpace(cleanComments))
                return delegationNote;

            return $"{cleanComments}\n\n{delegationNote}";
        }

        private async Task MarkReferenceRejectedAsync(
            Guid referenceId,
            ApprovalReferenceType referenceType)
        {
            if (referenceType == ApprovalReferenceType.Requisition)
            {
                var requisition = await _context.Requisitions
                    .FirstOrDefaultAsync(x => x.Id == referenceId);

                if (requisition == null)
                    throw new Exception("Requisition not found");

                requisition.Status = "Rejected";
                return;
            }

            if (referenceType == ApprovalReferenceType.RFQ)
            {
                var rfq = await _context.RequestForQuotations
                    .FirstOrDefaultAsync(x => x.Id == referenceId);

                if (rfq == null)
                    throw new Exception("RFQ not found");

                rfq.Status = RfqStatus.Rejected;
                return;
            }

            throw new InvalidOperationException("Unsupported approval reference type.");
        }

        private async Task MarkReferenceApprovedAsync(
            Guid referenceId,
            ApprovalReferenceType referenceType)
        {
            if (referenceType == ApprovalReferenceType.Requisition)
            {
                var requisition = await _context.Requisitions
                    .FirstOrDefaultAsync(x => x.Id == referenceId);

                if (requisition == null)
                    throw new Exception("Requisition not found");

                requisition.Status = "Approved";
                return;
            }

            if (referenceType == ApprovalReferenceType.RFQ)
            {
                var rfq = await _context.RequestForQuotations
                    .FirstOrDefaultAsync(x => x.RequisitionId == referenceId);

                if (rfq == null)
                    throw new Exception("RFQ not found");

                rfq.Status = RfqStatus.Awarded;
                return;
            }

            throw new InvalidOperationException("Unsupported approval reference type.");
        }
    }
}