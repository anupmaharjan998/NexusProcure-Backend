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

        public async Task ApproveRequisitionAsync(Guid requisitionId, Guid approverId, string decision, string comments)
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        // Get user's role
        var userRoleId = await _context.Users
            .Where(u => u.Id == approverId)
            .Select(u => u.RoleId)
            .FirstOrDefaultAsync();

        // Load requisition with items and approvals
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
            .Include(r => r.Approvals)
                .ThenInclude(a => a.ApprovedBy)
            .FirstOrDefaultAsync(r => r.Id == requisitionId);

        if (requisition == null)
            throw new KeyNotFoundException("Requisition not found");

        var totalAmount = requisition.Items.Sum(i => i.EstimatedCost);

        // Determine required approval levels for this amount
        var requiredLevels = await GetRequiredLevelsAsync(totalAmount);

        // Get the next required level that hasn't approved yet
        var nextLevel = requiredLevels
            .Where(l => !requisition.Approvals.Any(a => a.ApprovedBy.RoleId == l.RoleId))
            .FirstOrDefault();

        if (nextLevel == null)
            throw new InvalidOperationException("Requisition is already fully approved");

        if (nextLevel.RoleId != userRoleId)
            throw new UnauthorizedAccessException("You are not authorized to approve this requisition at this level");

        // Create new approval record
        var approval = new Approval
        {
            Id = Guid.NewGuid(),
            RequisitionId = requisitionId,
            ApprovedById = approverId,
            ApprovedDate = DateTime.UtcNow,
            Decision = decision,
            Comments = comments
        };

        _context.Approvals.Add(approval); // Ensure EF tracks it
        requisition.Approvals.Add(approval); // Optional: keep in-memory list updated

        // Update requisition status
        if (decision == "Rejected")
        {
            requisition.Status = "Rejected";
        }
        else if (decision == "Approved")
        {
            // Check if there are any remaining levels
            var remainingLevels = requiredLevels
                .Where(l => !requisition.Approvals.Any(a => a.ApprovedBy.RoleId == l.RoleId))
                .ToList();

            requisition.Status = remainingLevels.Any() ? "Partial Approved" : "Approved";
        }

        // Save changes and commit transaction
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