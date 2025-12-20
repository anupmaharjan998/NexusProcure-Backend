using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public RequisitionApprovalService(NexusProcureDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task ApproveRequisitionAsync(Guid requisitionId, Guid approverId, Guid roleId, string decision, string comments)
        {
            var requisition = await _context.Requisitions
                .Include(r => r.Items)
                .Include(r => r.Approvals)
                .FirstOrDefaultAsync(r => r.Id == requisitionId);

            if (requisition == null)
                throw new KeyNotFoundException("Requisition not found");

            var totalAmount = requisition.Items.Sum(i => i.EstimatedCost);

            // Check next required approval level
            var requiredLevels = await GetRequiredLevelsAsync(totalAmount);
            var nextLevel = requiredLevels
                .Where(l => !requisition.Approvals.Any(a => a.ApprovedBy.RoleId == l.RoleId))
                .FirstOrDefault();

            if (nextLevel == null)
                throw new InvalidOperationException("Requisition is already fully approved");

            if (nextLevel.RoleId != roleId)
                throw new UnauthorizedAccessException("You are not authorized to approve this requisition at this level");

            // Create approval record
            var approval = new Approval
            {
                Id = Guid.NewGuid(),
                RequisitionId = requisitionId,
                ApprovedById = approverId,
                ApprovedDate = DateTime.UtcNow,
                Decision = decision,
                Comments = comments
            };

            requisition.Approvals.Add(approval);

            // Update requisition status
            if (decision == "Rejected")
            {
                requisition.Status = "Rejected";
            }
            else if (decision == "Approved" && requisition.Approvals.Count == requiredLevels.Count)
            {
                requisition.Status = "Approved";
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ApprovalLevelResponseDto>> GetRequiredLevelsAsync(decimal amount)
        {
            var approvals = await _context.ApprovalLevels
                .Where(l => amount >= l.MinAmount && amount <= l.MaxAmount)
                .OrderBy(l => l.MinAmount)
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
            
            return _mapper.Map<List<ApprovalDto>>(requisition.Approvals.OrderBy(a => a.ApprovedDate).ToList());

            //return requisition?.Approvals.OrderBy(a => a.ApprovedDate).ToList() ?? new List<Approval>();
        }

        public async Task<List<RequisitionResponseDto>> GetPendingApprovalsForRoleAsync(Guid userId)
        {
            var userRoleId = await _context.Users.Where(u => u.Id == userId).Select(x => x.RoleId).FirstOrDefaultAsync();
            var requisitions = await _context.Requisitions
                .Include(r => r.Items)
                .Include(r => r.Approvals)
                .Where(r => r.Status == "Pending")
                .ToListAsync();

            var pendingForRole = new List<Requisition>();

            foreach (var r in requisitions)
            {
                var totalAmount = r.Items.Sum(i => i.EstimatedCost);
                var requiredLevels = await GetRequiredLevelsAsync(totalAmount);

                var nextLevel = requiredLevels
                    .Where(l => !r.Approvals.Any(a => a.ApprovedBy.RoleId == l.RoleId))
                    .FirstOrDefault();

                if (nextLevel != null && nextLevel.RoleId == userRoleId)
                    pendingForRole.Add(r);
            }

            return _mapper.Map<List<RequisitionResponseDto>>(pendingForRole);
        }
    }
}
