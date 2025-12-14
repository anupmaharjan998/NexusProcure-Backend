using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Procurement;

public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly NexusProcureDbContext _context;

        public PurchaseOrderService(NexusProcureDbContext context)
        {
            _context = context;
        }

        public async Task<List<PurchaseOrder>> GetAllAsync()
        {
            return await _context.PurchaseOrders
                .Include(po => po.Items)
                .Include(po => po.Vendor)
                .Include(po => po.Requisition)
                .ThenInclude(r => r.Items)
                .ToListAsync();
        }

        public async Task<PurchaseOrder> GetByIdAsync(Guid id)
        {
            var po = await _context.PurchaseOrders
                .Include(po => po.Items)
                .Include(po => po.Vendor)
                .Include(po => po.Requisition)
                .ThenInclude(r => r.Items)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (po == null)
                throw new KeyNotFoundException("Purchase order not found");

            return po;
        }

        public async Task<PurchaseOrder> CreateAsync(PurchaseOrderCreateDto dto)
        {
            // Ensure the requisition is approved
            var requisition = await _context.Requisitions
                .Include(r => r.Items)
                .FirstOrDefaultAsync(r => r.Id == dto.RequisitionId);

            if (requisition == null)
                throw new KeyNotFoundException("Requisition not found");

            if (requisition.Status != "Approved")
                throw new InvalidOperationException("Purchase orders can only be created from approved requisitions");

            var po = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                RequisitionId = dto.RequisitionId,
                VendorId = dto.VendorId,
                OrderDate = DateTime.UtcNow,
                Status = "Open",
                Items = dto.Items.Select(i => new PurchaseOrderItem
                {
                    Id = Guid.NewGuid(),
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();

            return po;
        }

        public async Task<PurchaseOrder> UpdateStatusAsync(Guid id, string status)
        {
            var po = await GetByIdAsync(id);

            if (!new[] { "Open", "Completed", "Cancelled" }.Contains(status))
                throw new ArgumentException("Invalid status value");

            po.Status = status;
            await _context.SaveChangesAsync();

            return po;
        }
    }