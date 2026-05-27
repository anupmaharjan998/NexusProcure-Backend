using Hangfire;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Procurement;

public class RequisitionService : IRequisitionService
{
    private readonly NexusProcureDbContext _context;
    private readonly IRiskScoringService _riskScoringService;
    private readonly IApprovalPolicyService _approvalPolicyService;

    public RequisitionService(
        NexusProcureDbContext context,
        IRiskScoringService riskScoringService, IApprovalPolicyService approvalPolicyService)
    {
        _context = context;
        _riskScoringService = riskScoringService;
        _approvalPolicyService = approvalPolicyService;
    }

    public async Task<IEnumerable<RequisitionResponseDto>> GetAllAsync()
    {
        var requisitions = await BaseQuery()
            .OrderByDescending(x => x.RequestedDate)
            .ToListAsync();

        return requisitions.Select(MapToResponseDto).ToList();
    }

    public async Task<RequisitionResponseDto> GetByIdAsync(Guid id)
    {
        var requisition = await BaseQuery()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (requisition == null)
        {
            throw new InvalidOperationException("Requisition not found.");
        }

        return MapToResponseDto(requisition);
    }

     public async Task<RequisitionResponseDto> CreateAsync(RequisitionCreateDto dto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        var committed = false;
        try
        {
            if (dto.RequestedById == Guid.Empty)
            {
                throw new InvalidOperationException("Requested by user is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Purpose))
            {
                throw new InvalidOperationException("Purpose is required.");
            }

            if (dto.Items == null || !dto.Items.Any())
            {
                throw new InvalidOperationException("At least one requisition item is required.");
            }

            var requestedByExists = await _context.Users
                .AnyAsync(u => u.Id == dto.RequestedById);

            if (!requestedByExists)
            {
                throw new InvalidOperationException("Requested by user not found.");
            }

            var duplicateStockIds = dto.Items
                .GroupBy(i => i.InventoryStockId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateStockIds.Any())
            {
                throw new InvalidOperationException(
                    "Duplicate inventory stock items are not allowed. Increase quantity instead.");
            }

            var stockIds = dto.Items
                .Select(i => i.InventoryStockId)
                .Distinct()
                .ToList();

            if (stockIds.Any(id => id == Guid.Empty))
            {
                throw new InvalidOperationException("Inventory stock is required for all items.");
            }

            var stocks = await _context.InventoryStocks
                .Include(s => s.Category)
                .Where(s => stockIds.Contains(s.Id))
                .ToListAsync();

            if (stocks.Count != stockIds.Count)
            {
                throw new InvalidOperationException("One or more inventory stock items were not found.");
            }

            var requisition = new Requisition
            {
                Id = Guid.NewGuid(),
                RequisitionNumber = await GenerateRequisitionNumberAsync(),
                RequestedById = dto.RequestedById,
                RequestedDate = DateTime.UtcNow,
                RequiredDate = dto.RequiredDate,
                Status = "Pending",
                IsUrgent = dto.IsUrgent,
                Purpose = dto.Purpose.Trim(),
                Notes = dto.Notes?.Trim(),
                RiskScore = 0,
                RiskLevel = RiskLevel.Low,
                TotalAmount = 0
            };
            if (dto.DepartmentId != Guid.Empty)
            {
                requisition.DepartmentId = dto.DepartmentId;
            }

            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Quantity <= 0)
                {
                    throw new InvalidOperationException("Quantity must be greater than zero.");
                }

                if (itemDto.EstimatedCost < 0)
                {
                    throw new InvalidOperationException("Estimated cost cannot be negative.");
                }

                var stock = stocks.First(s => s.Id == itemDto.InventoryStockId);

                var requisitionItem = new RequisitionItem
                {
                    Id = Guid.NewGuid(),
                    RequisitionId = requisition.Id,
                    InventoryStockId = stock.Id,
                    InventoryStock = stock,
                    Quantity = itemDto.Quantity,
                    EstimatedCost = itemDto.EstimatedCost,
                    Remarks = itemDto.Remarks?.Trim()
                };

                requisition.Items.Add(requisitionItem);

                requisition.TotalAmount += itemDto.Quantity * itemDto.EstimatedCost;
            }

            var riskScore = await _riskScoringService.CalculateRiskScoreAsync(requisition);
            requisition.RiskScore = riskScore;
            requisition.RiskLevel = _riskScoringService.ResolveRiskLevel(riskScore);

            _context.Requisitions.Add(requisition);

            var approvalLevels = await _approvalPolicyService.ResolveApprovalFlowAsync(requisition);

            if (approvalLevels == null || !approvalLevels.Any())
            {
                throw new InvalidOperationException("No approval policy configured.");
            }

            var firstSequenceOrder = approvalLevels.Min(a => a.SequenceOrder);

            foreach (var step in approvalLevels)
            {
                _context.Approvals.Add(new Approval
                {
                    Id = Guid.NewGuid(),
                    ReferenceId = requisition.Id,
                    ReferenceType = ApprovalReferenceType.Requisition,
                    RoleId = step.RoleId,
                    Status = "Pending",
                    SequenceOrder = step.SequenceOrder,
                    IsActive = step.SequenceOrder == firstSequenceOrder,
                    AssignedAt = DateTime.UtcNow,
                    Escalated = false
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
committed = true;
            BackgroundJob.Enqueue<IEmailJobService>(
                job => job.SendApprovalNotificationAsync(requisition.Id));

            return await GetByIdAsync(requisition.Id);
        }
        catch
        {
            if (!committed)
            {
                await transaction.RollbackAsync();
            }

            throw;
        }
    }

    public async Task<RequisitionResponseDto> ApproveAsync(
        Guid requisitionId,
        Guid approvedById,
        string comments)
    {
        var requisition = await _context.Requisitions
            .FirstOrDefaultAsync(x => x.Id == requisitionId);

        if (requisition == null)
        {
            throw new InvalidOperationException("Requisition not found.");
        }

        if (requisition.Status != "Pending")
        {
            throw new InvalidOperationException("Only pending requisitions can be approved.");
        }

        requisition.Status = "Approved";

        await _context.SaveChangesAsync();

        return await GetByIdAsync(requisitionId);
    }

    public async Task<RequisitionResponseDto> RejectAsync(
        Guid requisitionId,
        Guid rejectedById,
        string comments)
    {
        var requisition = await _context.Requisitions
            .FirstOrDefaultAsync(x => x.Id == requisitionId);

        if (requisition == null)
        {
            throw new InvalidOperationException("Requisition not found.");
        }

        if (requisition.Status != "Pending")
        {
            throw new InvalidOperationException("Only pending requisitions can be rejected.");
        }

        requisition.Status = "Rejected";

        await _context.SaveChangesAsync();

        return await GetByIdAsync(requisitionId);
    }

    private IQueryable<Requisition> BaseQuery()
    {
        return _context.Requisitions
            .Include(x => x.RequestedBy)
            .Include(x => x.Items)
                .ThenInclude(i => i.InventoryStock)
                    .ThenInclude(s => s.Category)
            .Include(x => x.Approvals)
                .ThenInclude(a => a.ApprovedBy)
            .AsNoTracking();
    }

    private RequisitionResponseDto MapToResponseDto(Requisition requisition)
    {
        var categoryNames = requisition.Items
            .Where(x => x.InventoryStock?.Category != null)
            .Select(x => x.InventoryStock.Category.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        var categoryName = categoryNames.Count switch
        {
            0 => "-",
            1 => categoryNames[0],
            _ => "Mixed"
        };

        return new RequisitionResponseDto
        {
            Id = requisition.Id,
            RequisitionNumber = requisition.RequisitionNumber,
            RequestedById = requisition.RequestedById,
            RequestedByName = requisition.RequestedBy.FullName,
            RequestedBy = new UserResponseDto
            {
                Id = requisition.RequestedBy.Id,
                FullName = requisition.RequestedBy.FullName,
                Email = requisition.RequestedBy.Email
            },
            RequestedDate = requisition.RequestedDate,
            RequiredDate = requisition.RequiredDate,
            Status = requisition.Status,
            IsUrgent = requisition.IsUrgent,
            Purpose = requisition.Purpose,
            Notes = requisition.Notes,
            TotalAmount = requisition.TotalAmount,
            RiskScore = requisition.RiskScore,
            RiskLevel = requisition.RiskLevel.ToString(),
            CategoryName = categoryName,
            Items = requisition.Items.Select(item => new RequisitionItemDto
            {
                Id = item.Id,
                RequisitionId = item.RequisitionId,
                InventoryStockId = item.InventoryStockId,
                ItemName = item.InventoryStock.Name,
                SKU = item.InventoryStock.SKU,
                CategoryName = item.InventoryStock.Category.Name,
                Quantity = item.Quantity,
                Unit = item.InventoryStock.Unit,
                EstimatedCost = item.EstimatedCost,
                LineTotal = item.Quantity * item.EstimatedCost,
                Remarks = item.Remarks
            }).ToList(),
            Approvals = requisition.Approvals?.Select(a => new ApprovalDto
            {
                Id = a.Id,
                Status = a.Status,
                Comments = a.Comments,
                ApprovedById = a.ApprovedById,
                ApprovedByName = a.ApprovedBy != null ? a.ApprovedBy.FullName : null,
                ActionedAt = a.ActionedAt
            }).ToList() ?? new List<ApprovalDto>()
        };
    }

    private async Task<string> GenerateRequisitionNumberAsync()
    {
        var year = DateTime.UtcNow.Year;

        var count = await _context.Requisitions
            .CountAsync(x => x.RequestedDate.Year == year);

        return $"REQ-{year}-{count + 1:D5}";
    }
}