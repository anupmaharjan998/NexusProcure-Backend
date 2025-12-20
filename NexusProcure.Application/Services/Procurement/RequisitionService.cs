using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Procurement;

public class RequisitionService : IRequisitionService
    {
        private readonly NexusProcureDbContext _context;
        private readonly IMapper _mapper;

        public RequisitionService(NexusProcureDbContext context,  IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RequisitionResponseDto>> GetAllAsync()
        {
            var requisitions = await _context.Requisitions
                .Include(r => r.RequestedBy)
                .Include(r => r.Items)
                .Include(r => r.Approvals)
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
                .Include(r => r.PurchaseOrders)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (requisition == null)
                throw new KeyNotFoundException("Requisition not found");

            return _mapper.Map<RequisitionResponseDto>(requisition);
        }

        public async Task<RequisitionResponseDto> CreateAsync(RequisitionCreateDto dto)
        {
            var requisition = new Requisition
            {
                Id = Guid.NewGuid(),
                RequestedById = dto.RequestedById,
                RequestedDate = DateTime.UtcNow,
                Status = "Pending",
                Items = dto.Items.Select(item => new RequisitionItem
                {
                    Id = Guid.NewGuid(),
                    ItemName = item.ItemName,
                    Quantity = item.Quantity,
                    EstimatedCost = item.EstimatedCost
                }).ToList()
            };

            await _context.Requisitions.AddAsync(requisition);
            await _context.SaveChangesAsync();

            return _mapper.Map<RequisitionResponseDto>(requisition);
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
                ApprovedDate = DateTime.UtcNow,
                Decision = "Approved",
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
                ApprovedDate = DateTime.UtcNow,
                Decision = "Rejected",
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