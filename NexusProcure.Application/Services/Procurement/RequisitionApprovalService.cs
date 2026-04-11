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
            // var approvals = await _context.Approvals
            //     .Include(a => a.Requisition)
            //     .ThenInclude(r => r.Items)
            //     .Where(a =>
            //         a.Status == "Pending" && a.IsActive && a.RoleId == user.RoleId)
            //     .Select(a => a.Requisition)
            //     .Distinct()
            //     .ToListAsync();

            if (user == null)
                throw new Exception("User not found");

            // Step 1: Get pending approvals for this role
            var requisitionIds = await _context.Approvals
                .Where(a =>
                    a.Status == "Pending" &&
                    a.IsActive &&
                    a.RoleId == user.RoleId &&
                    a.ReferenceType == ApprovalReferenceType.Requisition)
                .Select(a => a.ReferenceId)
                .Distinct()
                .ToListAsync();

            // Step 2: Load requisitions
            var requisitions = await _context.Requisitions
                .Include(r => r.Items)
                .Where(r => requisitionIds.Contains(r.Id))
                .ToListAsync();


            return _mapper.Map<List<RequisitionResponseDto>>(requisitions);
        }
        
        
        public async Task<List<QuotationApprovalListResponseDto>> GetPendingQuotationApprovalsForRoleAsync(Guid userId)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        
                    if (user == null)
                        throw new Exception("User not found");
        
                    // Step 1: Get pending approvals for this role
                    var approvalIds = await _context.Approvals
                        .Where(a =>
                            a.Status == "Pending" &&
                            a.IsActive &&
                            a.RoleId == user.RoleId &&
                            a.ReferenceType == ApprovalReferenceType.RFQ)
                        .Select(a => a.ReferenceId)
                        .Distinct()
                        .ToListAsync();
        
                    // Step 2: Load requisitions
                    var quotation = await _context.Quotations
                        .Include(r => r.RequestForQuotation)
                        .Include(v => v.RfqVendor.Vendor)
                        .Where(r => approvalIds.Contains(r.RfqId) && r.IsSelected)
                        .ToListAsync();
        
                    
                    return _mapper.Map<List<QuotationApprovalListResponseDto>>(quotation);
                }

        // public async Task ApproveAsync(
        //     Guid requisitionId,
        //     Guid approverId,
        //     string decision,
        //     string comments)
        // {
        //     await using var transaction =
        //         await _context.Database.BeginTransactionAsync();
        //
        //     bool sendStatusEmail = false;
        //     bool sendNextStepEmail = false;
        //     bool createRfq = false;
        //
        //     try
        //     {
        //         var user = await _context.Users
        //             .SingleAsync(u => u.Id == approverId);
        //
        //         var approval = await _context.Approvals
        //             .Include(a => a.Requisition)
        //             .FirstAsync(a =>
        //                 a.RequisitionId == requisitionId &&
        //                 a.RoleId == user.RoleId &&
        //                 a.Status == "Pending");
        //
        //         approval.ApprovedById = user.Id;
        //         approval.Status = decision;
        //         approval.Comments = comments;
        //         approval.ActionedAt = DateTime.UtcNow;
        //
        //         if (decision == "Rejected")
        //         {
        //             approval.Requisition.Status = "Rejected";
        //             sendStatusEmail = true;
        //         }
        //         else
        //         {
        //             bool stepCompleted = !await _context.Approvals.AnyAsync(a =>
        //                 a.RequisitionId == requisitionId &&
        //                 a.SequenceOrder == approval.SequenceOrder &&
        //                 a.Status == "Pending");
        //
        //             if (!stepCompleted)
        //             {
        //                 int nextStep = approval.SequenceOrder + 1;
        //
        //                 var nextApprovals = await _context.Approvals
        //                     .Where(a =>
        //                         a.RequisitionId == requisitionId &&
        //                         a.SequenceOrder == nextStep)
        //                     .ToListAsync();
        //
        //                 if (nextApprovals.Any())
        //                 {
        //                     foreach (var next in nextApprovals)
        //                     {
        //                         next.IsActive = true;
        //                     }
        //
        //                     sendNextStepEmail = true;
        //                 }
        //                 else
        //                 {
        //                     approval.Requisition.Status = "Approved";
        //                     sendStatusEmail = true;
        //                     createRfq = true;
        //                 }
        //             }
        //         }
        //
        //         await _context.SaveChangesAsync();
        //         await transaction.CommitAsync();
        //     }
        //     catch
        //     {
        //         await transaction.RollbackAsync();
        //         throw;
        //     }
        //
        //     // -----------------------------------
        //     // SIDE EFFECTS AFTER SUCCESSFUL COMMIT
        //     // -----------------------------------
        //
        //     if (sendStatusEmail)
        //     {
        //         BackgroundJob.Enqueue<IEmailJobService>(job => job.SendApprovalStatusEmailAsync(requisitionId));
        //     }
        //
        //     if (sendNextStepEmail)
        //     {
        //         BackgroundJob.Enqueue<IEmailJobService>(job => job.SendApprovalNotificationAsync(requisitionId));
        //     }
        //
        //     if (createRfq)
        //     {
        //         BackgroundJob.Enqueue<IRfqJob>(job => job.CreateAndSendRfqAsync(requisitionId));
        //     }
        // }


        public async Task ApproveAsync(
            Guid referenceId,
            Guid approverId,
            string decision,
            string comments,
            ApprovalReferenceType referenceType)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            bool sendStatusEmail = false;
            bool sendNextStepEmail = false;
            bool createRfq = false;
            bool createPurchaseRequest = false;
            var refId = referenceId;
            
            try
            {
                var user = await _context.Users
                    .SingleAsync(u => u.Id == approverId);

                
                if (referenceType == ApprovalReferenceType.RFQ)
                {
                    var rfqid = await _context.Quotations.Where(x => x.Id == referenceId).Select(r => r.RfqId).FirstOrDefaultAsync();
                    refId = rfqid;

                }
                var approval = await _context.Approvals
                    .FirstAsync(a =>
                        a.ReferenceId == refId &&
                        a.ReferenceType == referenceType &&
                        a.RoleId == user.RoleId &&
                        a.Status == "Pending" &&
                        a.IsActive);

                approval.ApprovedById = user.Id;
                approval.Status = decision;
                approval.Comments = comments;
                approval.ActionedAt = DateTime.UtcNow;
                approval.IsActive = false;

                if (decision == "Rejected")
                {
                    await UpdateReferenceStatus(refId, referenceType, "Rejected");
                    sendStatusEmail = true;
                }
                else
                {
                    bool stepCompleted = !await _context.Approvals.AnyAsync(a =>
                        a.ReferenceId == refId &&
                        a.ReferenceType == referenceType &&
                        a.SequenceOrder == approval.SequenceOrder &&
                        a.Status == "Pending");

                    if (!stepCompleted)
                    {
                        int nextStep = approval.SequenceOrder + 1;

                        var nextApprovals = await _context.Approvals
                            .Where(a =>
                                a.ReferenceId == refId &&
                                a.ReferenceType == referenceType &&
                                a.SequenceOrder == nextStep)
                            .ToListAsync();

                        if (nextApprovals.Any())
                        {
                            foreach (var next in nextApprovals)
                            {
                                if (next.SequenceOrder == nextStep)
                                    next.IsActive = true;
                            }

                            sendNextStepEmail = true;
                        }
                        else
                        {
                            // Final Approval
                            await UpdateReferenceStatus(refId, referenceType, "Approved");
                            sendStatusEmail = true;

                            if (referenceType == ApprovalReferenceType.Requisition)
                                createRfq = true;

                            if (referenceType == ApprovalReferenceType.RFQ)
                                createPurchaseRequest = true;
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

            // ---------------------------
            // Background Jobs (Post Commit)
            // ---------------------------

            if (sendStatusEmail)
            {
                if (referenceType == ApprovalReferenceType.Requisition)
                {
                    BackgroundJob.Enqueue<IEmailJobService>(job =>
                        job.SendApprovalStatusEmailAsync(refId));
                }
                else
                {
                    BackgroundJob.Enqueue<IEmailJobService>(job =>
                        job.SendQuotationApprovalEmailAsync(refId));
                }
            }

            if (sendNextStepEmail)
            {
                if (referenceType == ApprovalReferenceType.Requisition)
                {
                    BackgroundJob.Enqueue<IEmailJobService>(job =>
                        job.SendApprovalNotificationAsync(refId));
                }
                
            }

            if (createRfq)
            {
                if (referenceType == ApprovalReferenceType.Requisition)
                {
                    BackgroundJob.Enqueue<IRfqJob>(job =>
                        job.CreateAndSendRfqAsync(refId));
                }
                
            }

            if (createPurchaseRequest)
            {
                BackgroundJob.Enqueue<IPurchaseRequestJob>(job => job.CreatePurchaseRequestAsync(refId));
            }
        }


        private async Task UpdateReferenceStatus(
            Guid referenceId,
            ApprovalReferenceType referenceType,
            string status)
        {
            if (referenceType == ApprovalReferenceType.Requisition)
            {
                var requisition = await _context.Requisitions
                    .FirstAsync(r => r.Id == referenceId);

                requisition.Status = status;
            }

            if (referenceType == ApprovalReferenceType.RFQ)
            {
                var rfq = await _context.RequestForQuotations
                    .FirstAsync(r => r.Id == referenceId);
                if (status == "Approved")
                {
                    rfq.Status = RfqStatus.Awarded;
                }
                else
                {
                    rfq.Status = Enum.Parse<RfqStatus>(status);
                }
            }

            // if (referenceType == ApprovalReferenceType.PurchaseRequest)
            // {
            //     var pr = await _context.PurchaseRequests
            //         .FirstAsync(p => p.Id == referenceId);
            //
            //     pr.Status = status;
            // }
        }
    }
}