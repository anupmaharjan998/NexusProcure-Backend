using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Helper;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Core.DTOs.PurchaseOrder;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Procurement;

public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly NexusProcureDbContext _context;
        private readonly IPurchaseOrderNumberGenerator _purchaseOrderNumberGenerator;

        public PurchaseOrderService(NexusProcureDbContext context, IPurchaseOrderNumberGenerator purchaseOrderNumberGenerator)
        {
            _context = context;
            _purchaseOrderNumberGenerator = purchaseOrderNumberGenerator;
        }

        public async Task<PurchaseOrderListResponse> GetAllAsync()
        {
            var purchaseOrders = await _context.PurchaseOrders
                .Include(x => x.Vendor)
                .ToListAsync();

            var response = new PurchaseOrderListResponse
            {
                TotalPOs = purchaseOrders.Count,

                TotalValue = purchaseOrders.Sum(x => x.TotalAmount),

                InTransit = purchaseOrders.Count(x => x.DeliveryStatus == DeliveryStatus.Pending),

                Delivered = purchaseOrders.Count(x => x.DeliveryStatus == DeliveryStatus.Received),

                Orders = purchaseOrders.Select(po => new PurchaseOrderDto
                {
                    Id = po.Id,
                    PoNumber = po.PurchaseOrderNumber,
                    VendorName = po.Vendor.VendorName,
                    PoDate = po.OrderDate,
                    DeliveryDate = po.DeliveryDate,
                    Status = po.Status.ToString(),
                    DeliveryStatus = po.DeliveryStatus.ToString(),
                    TotalAmount = po.TotalAmount
                }).ToList()
            };

            return response;
        }

        // public async Task<PurchaseOrder> GetByIdAsync(Guid id)
        // {
        //     var po = await _context.PurchaseOrders
        //         .Include(po => po.Items)
        //         .Include(po => po.Vendor)
        //         .Include(po => po.Requisition)
        //         .ThenInclude(r => r.Items)
        //         .FirstOrDefaultAsync(po => po.Id == id);
        //
        //     if (po == null)
        //         throw new KeyNotFoundException("Purchase order not found");
        //
        //     return po;
        // }
        
        public async Task<PurchaseOrderDto> GetByIdAsync(Guid id)
        {
            var po = await _context.PurchaseOrders
                .Include(x => x.Requisition)
                .Include(x => x.Items)
                .Include(v => v.Vendor)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (po == null) return null;

            return new PurchaseOrderDto
            {
                Id = po.Id,
                PoNumber = po.PurchaseOrderNumber,
                ReqNumber = po.Requisition.RequisitionNumber,
                VendorName = po.Vendor.CompanyName,
                VendorEmail = po.Vendor.Email,
                VendorPhoneNumber = po.Vendor.PhoneNumber,
                VendorAddress = po.Vendor.Address,
                VendorContactPerson = po.Vendor.VendorName,
                PaymentTerms = po.Vendor.PaymentTerms.ToString(),
                
                PoDate = po.OrderDate,
                DeliveryDate = po.DeliveryDate,
                Status = po.Status.ToString(),
                DeliveryStatus = po.DeliveryStatus.ToString(),
                SubTotal = po.SubTotal,
                Vat = po.Vat,
                TotalAmount = po.TotalAmount,
                Items = po.Items.Select(i => new PurchaseOrderItemDto
                {
                    ItemName = i.ItemName,
                    TaxPercentage = i.TaxPercentage,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    LineTotal = (i.Quantity * i.UnitPrice) + ((i.Quantity * i.UnitPrice) * (i.TaxPercentage)/100),
                }).ToList()
            };
        }

        public async Task<PurchaseOrder> CreateAsync(Guid referenceId)
        {
            var rfq = await _context.RequestForQuotations
                .Include(r => r.Vendors)
                .ThenInclude(v => v.Vendor)
                .Include(r => r.Quotations)
                .ThenInclude(q => q.Items)
                .FirstOrDefaultAsync(x => x.Id == referenceId);

            if (rfq == null)
                throw new KeyNotFoundException("Requisition not found");
            
            var existingPo = await _context.PurchaseOrders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(r => r.RequisitionId == rfq.Id);

            if (existingPo != null)
            {
                return existingPo;
            }

            if (rfq.Status != RfqStatus.Awarded)
                throw new InvalidOperationException("Purchase orders can only be created from awarded requisitions");

            var quotation = rfq.Quotations.FirstOrDefault(x => x.IsSelected);
            var vendorId = rfq.Vendors.FirstOrDefault(x => x.Id == quotation.RfqVendorId);
            if (quotation == null)
                throw new InvalidOperationException("No selected quotation found");

            var po = new PurchaseOrder
            {
                Id = Guid.NewGuid(),
                PurchaseOrderNumber = await _purchaseOrderNumberGenerator.GeneratePoNumberAsync(),
                RequisitionId = rfq.RequisitionId,
                VendorId = vendorId.Vendor.Id,
                OrderDate = DateTime.UtcNow,
                Status = PurchaseOrderStatus.Issued,
                QuotationId = quotation.Id,
                CreatedAt = DateTime.UtcNow,
                DeliveryDate = quotation.DeliveryDate,
                TotalAmount = quotation.TotalAmount,
                Items = quotation.Items.Select(i => new PurchaseOrderItem
                {
                    Id = Guid.NewGuid(),
                    ItemName = i.ItemName,
                    TaxPercentage = i.TaxPercentage,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            _context.PurchaseOrders.Add(po);
            await _context.SaveChangesAsync();

            return po;
        }

        public async Task<PurchaseOrderDto> UpdateStatusAsync(Guid id, string status)
        {
            var po = await GetByIdAsync(id);

            if (!new[] { "Open", "Completed", "Cancelled" }.Contains(status))
                throw new ArgumentException("Invalid status value");

            po.Status = PurchaseOrderStatus.Completed.ToString();
            await _context.SaveChangesAsync();

            return po;
        }
    }