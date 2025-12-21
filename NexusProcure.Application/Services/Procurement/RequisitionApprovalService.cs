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

        public async Task ApproveRequisitionAsync(
            Guid requisitionId,
            Guid approverId,
            string decision,
            string comments)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users
                    .Where(u => u.Id == approverId)
                    .Select(u => new { u.Id, u.RoleId })
                    .FirstOrDefaultAsync();

                if (user == null)
                    throw new UnauthorizedAccessException("Invalid approver");

                var requisition = await _context.Requisitions
                    .Include(r => r.Items)
                    .Include(r => r.Approvals)
                    .FirstOrDefaultAsync(r => r.Id == requisitionId);

                if (requisition == null)
                    throw new KeyNotFoundException("Requisition not found");

                var totalAmount = requisition.Items.Sum(i => i.EstimatedCost);
                var requiredLevels = await GetRequiredLevelsAsync(totalAmount);

                // Resolve roleIds that already approved
                var approvedRoleIds = await _context.Approvals
                    .Where(a => a.RequisitionId == requisitionId)
                    .Join(
                        _context.Users,
                        a => a.ApprovedById,
                        u => u.Id,
                        (a, u) => u.RoleId
                    )
                    .ToListAsync();

                var nextLevel = requiredLevels
                    .FirstOrDefault(l => !approvedRoleIds.Contains(l.RoleId));

                if (nextLevel == null)
                    throw new InvalidOperationException("Requisition already fully approved");

                if (nextLevel.RoleId != user.RoleId)
                    throw new UnauthorizedAccessException("You are not authorized for this approval level");

                var approval = new Approval
                {
                    Id = Guid.NewGuid(),
                    RequisitionId = requisitionId,
                    ApprovedById = approverId,
                    ApprovedDate = DateTime.UtcNow,
                    Decision = decision,
                    Comments = comments
                };

                _context.Approvals.Add(approval);

                if (decision == "Rejected")
                {
                    requisition.Status = "Rejected";
                }
                else
                {
                    approvedRoleIds.Add(user.RoleId);

                    var remainingLevels = requiredLevels
                        .Any(l => !approvedRoleIds.Contains(l.RoleId));

                    requisition.Status = remainingLevels
                        ? "Partial Approved"
                        : "Approved";
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


        public async Task<List<ApprovalLevelResponseDto>> GetRequiredLevelsAsync(decimal amount)
        {
            var approvals = await _context.ApprovalLevels
                .Where(l => amount >= l.MinAmount)
                .OrderBy(l => l.MinAmount) // ensures hierarchy
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
            var userRoleId =
                await _context.Users.Where(u => u.Id == userId).Select(x => x.RoleId).FirstOrDefaultAsync();
            var requisitions = await _context.Requisitions
                .Include(r => r.Items)
                .Include(r => r.Approvals)
                .ThenInclude(a => a.ApprovedBy)
                .Where(r => r.Status == "Pending" || r.Status == "Partial Approved")
                .ToListAsync();

            var pendingForRole = new List<Requisition>();

            foreach (var r in requisitions)
            {
                var totalAmount = r.Items.Sum(i => i.EstimatedCost);
                var requiredLevels = await GetRequiredLevelsAsync(totalAmount);

                // var nextLevel = requiredLevels
                //     .Where(l => !r.Approvals.Any(a => a.ApprovedBy.RoleId == l.RoleId))
                //     .FirstOrDefault();
                var nextLevel = requiredLevels.FirstOrDefault(level =>
                    !r.Approvals.Any(a => a.ApprovedBy.RoleId == level.RoleId)
                );

                if (nextLevel != null && nextLevel.RoleId == userRoleId)
                    pendingForRole.Add(r);
            }

            return _mapper.Map<List<RequisitionResponseDto>>(pendingForRole);
        }
    }
}