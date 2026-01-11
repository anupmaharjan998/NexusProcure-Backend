using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.Helper;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Procurement;

public class RequisitionService : IRequisitionService
{
    private readonly NexusProcureDbContext _context;
    private readonly IMapper _mapper;
    private readonly IRiskScoringService _riskScoringService;
    private readonly IApprovalPolicyService _approvalPolicyService;
    private readonly IRequisitionNumberGenerator _requisitionNumberGenerator;

    public RequisitionService(NexusProcureDbContext context, IMapper mapper, IRiskScoringService riskScoringService,
        IApprovalPolicyService approvalPolicyService, IRequisitionNumberGenerator requisitionNumberGenerator)
    {
        _context = context;
        _mapper = mapper;
        _riskScoringService = riskScoringService;
        _approvalPolicyService = approvalPolicyService;
        _requisitionNumberGenerator = requisitionNumberGenerator;
    }

    public async Task<IEnumerable<RequisitionResponseDto>> GetAllAsync()
    {
        var requisitions = await _context.Requisitions
            .Include(r => r.RequestedBy)
            .Include(r => r.Items)
            .Include(r => r.Approvals)
            .Include(r => r.Category)
            .Include(r => r.PurchaseOrders)
            .ThenInclude(po => po.Items)
            .ToListAsync();

        return _mapper.Map<IEnumerable<RequisitionResponseDto>>(requisitions);
    }

    public async Task<RequisitionResponseDto> GetByIdAsync(Guid id)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.RequestedBy)
            .Include(r => r.Items)
            .Include(r => r.Approvals)
                .ThenInclude(a => a.ApprovedBy)
                    .ThenInclude(u => u.Role)
            .Include(r => r.PurchaseOrders)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (requisition == null)
            throw new KeyNotFoundException("Requisition not found");


        return _mapper.Map<RequisitionResponseDto>(requisition);
    }

    public async Task<RequisitionResponseDto> CreateAsync(RequisitionCreateDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var requisition = new Requisition
            {
                Id = Guid.NewGuid(),
                RequisitionNumber = await _requisitionNumberGenerator.GenerateRequisitionNumberAsync(),
                RequestedById = dto.RequestedById,
                RequestedDate = DateTime.UtcNow,
                Status = "Pending",
                CategoryId = dto.CategoryId,
                IsUrgent = dto.IsUrgent,
                Items = dto.Items.Select(item => new RequisitionItem
                {
                    Id = Guid.NewGuid(),
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    EstimatedCost = item.EstimatedCost
                }).ToList()
            };

            // 🔹 Risk scoring
            var riskScore = await _riskScoringService.CalculateRiskScoreAsync(requisition);
            requisition.RiskScore = riskScore;
            //requisition.RiskLevel = _riskScoringService.ResolveRiskLevel(riskScore);
            requisition.RiskLevel = riskScore switch
            {
                >= 70 => RiskLevel.Critical,
                >= 50 => RiskLevel.High,
                >= 30 => RiskLevel.Medium,
                _ => RiskLevel.Low
            };

            await _context.Requisitions.AddAsync(requisition);
            await _context.SaveChangesAsync();

            // 🔹 Resolve approval flow
            var approvalLevels =
                await _approvalPolicyService.ResolveApprovalFlowAsync(requisition);

            if (!approvalLevels.Any())
                throw new InvalidOperationException("No approval policy configured");

            //var firstLevel = approvalLevels.First();

            // 🔹 Assign first approver (role → user)
            // var approverUserId = await _context.Users
            //     .Where(u => u.RoleId == firstLevel.Id)
            //     .Select(u => u.Id)
            //     .FirstOrDefaultAsync();
            //
            // if (approverUserId == Guid.Empty)
            //     throw new InvalidOperationException("No approver found for role");
            //
            // var approval = new Approval
            // {
            //     Id = Guid.NewGuid(),
            //     RequisitionId = requisition.Id,
            //     RoleId = firstLevel.Id,
            //     AssignedToUserId = approverUserId,
            //     AssignedAt = DateTime.UtcNow,
            //     Status = "Pending"
            // };

            foreach (var step in approvalLevels)
            {
                _context.Approvals.Add(new Approval
                {
                    Id = Guid.NewGuid(),
                    RequisitionId = requisition.Id,
                    RoleId = step.RoleId,
                    Status = "Pending",
                    SequenceOrder = step.SequenceOrder,
                    IsActive = step.SequenceOrder == 1,
                    AssignedAt = DateTime.UtcNow
                });
            }
            
            //await _context.Approvals.AddAsync(approval);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return _mapper.Map<RequisitionResponseDto>(requisition);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<RequisitionResponseDto> ApproveAsync(Guid requisitionId, Guid approvedById, string comments)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
            .Include(r => r.Approvals)
            .Include(r => r.PurchaseOrders)
            .FirstOrDefaultAsync(r => r.Id == requisitionId);

        if (requisition == null)
            throw new KeyNotFoundException("Requisition not found.");

        if (requisition.Status != "Pending")
            throw new InvalidOperationException("Only pending requisitions can be approved");

        var approval = new Approval
        {
            Id = Guid.NewGuid(),
            RequisitionId = requisitionId,
            ApprovedById = approvedById,
            Comments = comments
        };

        requisition.Status = "Approved";
        requisition.Approvals.Add(approval);

        await _context.SaveChangesAsync();

        return _mapper.Map<RequisitionResponseDto>(requisition);
    }


    public async Task<RequisitionResponseDto> RejectAsync(Guid requisitionId, Guid rejectedById, string comments)
    {
        // Fetch requisition with related data
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
            .Include(r => r.Approvals)
            .Include(r => r.PurchaseOrders)
            .FirstOrDefaultAsync(r => r.Id == requisitionId);

        if (requisition == null)
            throw new KeyNotFoundException("Requisition not found.");

        if (requisition.Status != "Pending")
            throw new InvalidOperationException("Only pending requisitions can be rejected");

        var approval = new Approval
        {
            Id = Guid.NewGuid(),
            RequisitionId = requisitionId,
            ApprovedById = rejectedById,
            Comments = comments
        };

        requisition.Status = "Rejected";
        requisition.Approvals.Add(approval);

        await _context.SaveChangesAsync();

        // Map the updated requisition to DTO
        var requisitionDto = _mapper.Map<RequisitionResponseDto>(requisition);
        return requisitionDto;
    }
}