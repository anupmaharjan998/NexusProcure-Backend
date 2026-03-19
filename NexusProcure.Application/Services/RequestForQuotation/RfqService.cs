using System.Security.Cryptography;
using AutoMapper;
using ClosedXML.Excel;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.Helper;
using NexusProcure.Application.Interfaces.RequestForQuotation;
using NexusProcure.Core.DTOs.RFQ;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Entities.RequestForQuotations;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.RequestForQuotation;

public class RfqService : IRfqService
{
    private readonly NexusProcureDbContext _context;
    private readonly IRfqNumberGenerator _rfqNumberGenerator;
    private readonly IMapper _mapper;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public RfqService(NexusProcureDbContext context, IRfqNumberGenerator rfqNumberGenerator, IMapper mapper, IBackgroundJobClient backgroundJobClient)
    {
        _context = context;
        _rfqNumberGenerator = rfqNumberGenerator;
        _mapper = mapper;
        _backgroundJobClient = backgroundJobClient;
    }


    public async Task<Core.Entities.RequestForQuotations.RequestForQuotation> CreateRfqAsync(Guid requisitionId)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var existingRfq = await _context.RequestForQuotations
                .FirstOrDefaultAsync(r => r.RequisitionId == requisitionId);

            if (existingRfq != null)
            {
                return existingRfq;
            }
            var requisition = await _context.Requisitions
                .Include(x => x.Items)
                .FirstAsync(x => x.Id == requisitionId);

            var vendors = await _context.Vendors
                .Where(v =>
                    v.Status == "Active" &&
                    v.CategoryId == requisition.CategoryId)
                .ToListAsync();

            if (!vendors.Any())
                throw new InvalidOperationException("No eligible vendors found.");

            var rfq = new Core.Entities.RequestForQuotations.RequestForQuotation
            {
                Id = Guid.NewGuid(),
                RfqNumber = await _rfqNumberGenerator.GenerateRfqNumberAsync(),
                RequisitionId = requisition.Id,
                SubmissionDeadline = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                Status = RfqStatus.Open
            };

            _context.RequestForQuotations.Add(rfq);

            foreach (var vendor in vendors)
            {
                var token = await GenerateAccessTokenAsync(rfq.Id, vendor.Id);
                // ✅ INVITATION (CRITICAL)
                var rfqVendor = new RfqVendor
                {
                    Id = Guid.NewGuid(),
                    RfqId = rfq.Id,
                    VendorId = vendor.Id,
                    AccessToken = token.Token,
                    TokenExpiresAt = token.ExpiresAt,
                };

                _context.RfqVendors.Add(rfqVendor);

                // ✅ TOKEN (AFTER INVITATION)
            }

            _context.RfqAudits.Add(new RfqAudit
            {
                Id = Guid.NewGuid(),
                RfqId = rfq.Id,
                Action = "Created",
                CreatedAt = DateTime.UtcNow,
                PerformedBy = "System"
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();

            return rfq;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }


    public async Task<PublicRfqDto?> GetRfqByTokenAsync(string token)
    {
        var access = await _context.RfqAccessTokens
            .Include(v => v.Vendor)
            .Include(x => x.Rfq)
            .ThenInclude(x => x.Requisition)
            .ThenInclude(x => x.Items)
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);

        if (access == null)
            return null;

        _context.RfqAudits.Add(new RfqAudit
        {
            Id = Guid.NewGuid(),
            RfqId = access.RfqId,
            Action = "VendorAccessed",
            CreatedAt = DateTime.UtcNow,
            PerformedBy = access.VendorId.ToString()
        });


        return new PublicRfqDto
        {
            RfqNumber = access.Rfq.RfqNumber,
            CreatedAt = access.Rfq.CreatedAt,
            Vendor = new RfqVendorDto()
            {
                CompanyName = access.Vendor.CompanyName,
                Address = access.Vendor.Address,
                VendorId = access.Vendor.Id,
                VendorName = access.Vendor.VendorName,
                Phone = access.Vendor.PhoneNumber,
                Email = access.Vendor.Email,
                PaymentTerms = access.Vendor.PaymentTerms.ToString()
            },
            SubmissionDeadline = access.Rfq.SubmissionDeadline,
            Items = access.Rfq.Requisition.Items.Select(i => new PublicRfqItemDto()
            {
                ItemName = i.ItemName,
                Quantity = i.Quantity
            }).ToList()
        };
    }


    public async Task<RfqAccessToken> GenerateAccessTokenAsync(Guid rfqId, Guid vendorId)
    {
        var existing = await _context.RfqAccessTokens
            .FirstOrDefaultAsync(x =>
                x.RfqId == rfqId &&
                x.VendorId == vendorId &&
                x.ExpiresAt > DateTime.UtcNow);

        if (existing != null)
            return existing;

        var token = new RfqAccessToken
        {
            Id = Guid.NewGuid(),
            RfqId = rfqId,
            VendorId = vendorId,
            Token = SecureTokenGenerator.Generate(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.RfqAccessTokens.Add(token);
        return token;
    }


    public async Task<RfqTokenDto> ValidateRfqTokenAsync(string token)
    {
        var access = await _context.RfqAccessTokens
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);

        if (access == null)
            throw new InvalidOperationException("Invalid or expired RFQ token.");

        return new RfqTokenDto
        {
            RfqId = access.RfqId,
            ExpiresAt = access.ExpiresAt,
            IsUsed = access.IsUsed,
        };
    }


    public async Task SubmitQuotationAsync(
        string token,
        QuotationSubmitDto dto,
        string? ipAddress)
    {
        var access = await ValidateTokenAsync(token);

        if (access.Rfq.Status != RfqStatus.Open)
            throw new InvalidOperationException("RFQ is not accepting quotations.");

        if (access.Rfq.SubmissionDeadline < DateTime.UtcNow)
            throw new InvalidOperationException("Submission deadline has passed.");

        var rfqVendor = await _context.RfqVendors
            .FirstOrDefaultAsync(rv =>
                rv.RfqId == access.RfqId &&
                rv.VendorId == access.VendorId);

        if (rfqVendor == null)
            throw new InvalidOperationException("Vendor is not invited to this RFQ.");

        bool alreadySubmitted = await _context.Quotations.AnyAsync(q =>
            q.RfqId == access.RfqId &&
            q.RfqVendorId == rfqVendor.Id);

        if (alreadySubmitted)
            throw new InvalidOperationException("Quotation already submitted.");

        // 🔒 Load original RFQ items
        var rfqItems = await _context.Requisitions
            .Where(r => r.Id == access.Rfq.RequisitionId)
            .SelectMany(r => r.Items)
            .ToListAsync();

        if (dto.Items.Count != rfqItems.Count)
            throw new InvalidOperationException("RFQ items were tampered.");

        var quotation = new Quotation
        {
            Id = Guid.NewGuid(),
            RfqId = access.RfqId,
            RfqVendorId = rfqVendor.Id,
            SubmittedAt = DateTime.UtcNow,
            Notes = dto.Notes ?? "",
            SignedBy = dto.Signature,
            IpAddress = ipAddress ?? "Unknown",
            DeliveryDate = dto.DeliveryTime.ToUniversalTime(),
            SubmissionMethod = "Web Interface",
            
        };

        decimal totalAmount = 0;

        foreach (var item in dto.Items)
        {
            var original = rfqItems.FirstOrDefault(i =>
                i.ItemName == item.ItemName);

            if (original == null)
                throw new InvalidOperationException("Invalid RFQ item detected.");

            if (original.Quantity != item.Quantity)
                throw new InvalidOperationException(
                    $"Quantity tampering detected for item '{item.ItemName}'.");

            var lineTotal =
                (item.UnitPrice * item.Quantity) +
                (item.UnitPrice * item.Quantity * item.TaxPercentage / 100m);

            totalAmount += lineTotal;

            _context.QuotationItems.Add(new QuotationItem
            {
                Id = Guid.NewGuid(),
                QuotationId = quotation.Id,
                ItemName = item.ItemName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TaxPercentage = item.TaxPercentage
            });
        }

        quotation.TotalAmount = totalAmount;
        _context.Quotations.Add(quotation);

        access.IsUsed = true;

        _context.RfqAudits.Add(new RfqAudit
        {
            Id = Guid.NewGuid(),
            RfqId = access.RfqId,
            Action = "QuoteSubmitted",
            CreatedAt = DateTime.UtcNow,
            PerformedBy = access.VendorId.ToString()
        });

        await _context.SaveChangesAsync();
    }


//     public async Task SubmitQuotationFromExcelAsync(string token, IFormFile file)
// {
//     await using var tx = await _context.Database.BeginTransactionAsync();
//
//     try
//     {
//         var access = await ValidateTokenAsync(token);
//
//         var rfqVendor = await _context.RfqVendors
//             .FirstOrDefaultAsync(rv =>
//                 rv.RfqId == access.RfqId &&
//                 rv.VendorId == access.VendorId);
//
//         if (rfqVendor == null)
//             throw new InvalidOperationException("Vendor is not invited to this RFQ.");
//
//         if (await _context.Quotations.AnyAsync(q =>
//                 q.RfqId == access.RfqId &&
//                 q.RfqVendorId == rfqVendor.Id))
//             throw new InvalidOperationException("Quotation already submitted.");
//
//         using var stream = new MemoryStream();
//         await file.CopyToAsync(stream);
//         using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
//         var ws = workbook.Worksheet(1);
//
//         var quotation = new Quotation
//         {
//             Id = Guid.NewGuid(),
//             RfqId = access.RfqId,
//             RfqVendorId = rfqVendor.Id,
//             SubmittedAt = DateTime.UtcNow
//         };
//
//         _context.Quotations.Add(quotation);
//
//         int row = 5; // matches your template
//         while (!string.IsNullOrWhiteSpace(ws.Cell(row, 1).GetString()))
//         {
//             decimal unitPrice = ws.Cell(row, 3).TryGetValue(out decimal up) ? up : 0;
//             decimal vat = ws.Cell(row, 4).TryGetValue(out decimal v) ? v : 0;
//
//             _context.QuotationItems.Add(new QuotationItem
//             {
//                 Id = Guid.NewGuid(),
//                 QuotationId = quotation.Id,
//                 ItemName = ws.Cell(row, 1).GetString(),
//                 UnitPrice = unitPrice,
//                 TaxPercentage = vat
//             });
//
//             row++;
//         }
//
//
//         access.IsUsed = true;
//
//         _context.RfqAudits.Add(new RfqAudit
//         {
//             Id = Guid.NewGuid(),
//             RfqId = access.RfqId,
//             Action = "ExcelQuotationUploaded",
//             CreatedAt = DateTime.UtcNow,
//             PerformedBy = access.VendorId.ToString()
//         });
//
//         await _context.SaveChangesAsync();
//         await tx.CommitAsync();
//     }
//     catch
//     {
//         await tx.RollbackAsync();
//         throw;
//     }
// }
    public async Task SubmitQuotationFromExcelAsync(string token, IFormFile file)
    {
        var access = await ValidateTokenAsync(token);

        if (access.Rfq.SubmissionDeadline < DateTime.UtcNow)
            throw new InvalidOperationException("Submission deadline has passed.");

        var rfqVendor = await _context.RfqVendors
            .FirstOrDefaultAsync(rv =>
                rv.RfqId == access.RfqId &&
                rv.VendorId == access.VendorId);

        if (rfqVendor == null)
            throw new InvalidOperationException("Vendor is not invited to this RFQ.");

        bool alreadySubmitted = await _context.Quotations.AnyAsync(q =>
            q.RfqVendorId == rfqVendor.Id);

        if (alreadySubmitted)
            throw new InvalidOperationException("Quotation already submitted.");

        // 🔒 Load original RFQ items
        var rfqItems = await _context.Requisitions
            .Where(r => r.Id == access.Rfq.RequisitionId)
            .SelectMany(r => r.Items)
            .ToListAsync();

        if (!rfqItems.Any())
            throw new InvalidOperationException("RFQ has no items.");

        // 📘 Read Excel
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet(1);

        var excelItems = new List<(string ItemName, int Quantity, decimal UnitPrice, decimal Vat)>();

        int row = 5; // first item row (based on your template)

        while (!ws.Cell(row, 1).IsEmpty())
        {
            excelItems.Add((
                ItemName: ws.Cell(row, 1).GetString(),
                Quantity: ws.Cell(row, 2).GetValue<int>(),
                UnitPrice: ws.Cell(row, 3).GetValue<decimal>(),
                Vat: ws.Cell(row, 4).GetValue<decimal>()
            ));

            row++;
        }

        // 🔒 VALIDATION: row count
        if (excelItems.Count != rfqItems.Count)
            throw new InvalidOperationException("RFQ items have been tampered.");

        // 🔒 VALIDATION: item + quantity
        foreach (var excelItem in excelItems)
        {
            var original = rfqItems.FirstOrDefault(i =>
                i.ItemName == excelItem.ItemName);

            if (original == null)
                throw new InvalidOperationException($"Invalid item '{excelItem.ItemName}' detected.");

            if (original.Quantity != excelItem.Quantity)
                throw new InvalidOperationException(
                    $"Quantity tampering detected for item '{original.ItemName}'.");
        }

        // ✅ Save quotation
        var quotation = new Quotation
        {
            Id = Guid.NewGuid(),
            RfqId = access.RfqId,
            RfqVendorId = rfqVendor.Id,
            SubmittedAt = DateTime.UtcNow
        };

        _context.Quotations.Add(quotation);

        foreach (var item in excelItems)
        {
            _context.QuotationItems.Add(new QuotationItem
            {
                Id = Guid.NewGuid(),
                QuotationId = quotation.Id,
                ItemName = item.ItemName,
                UnitPrice = item.UnitPrice,
                TaxPercentage = item.Vat
            });
        }

        access.IsUsed = true;

        await _context.SaveChangesAsync();
    }

    public async Task<List<RfqDto>> GetAllRfqAsync()
    {
        var rfqs = await _context.RequestForQuotations
            .Include(r => r.Quotations) // only for Count
            .AsNoTracking()
            .OrderByDescending(x => x.RfqNumber)
            .ToListAsync();

        return _mapper.Map<List<RfqDto>>(rfqs);
    }


    public async Task<QuotationListResponseDto> GetQuotationByRfqIdAsync(Guid rfqId)
    {
        var quotations = await _context.Quotations
            .Include(x => x.RfqVendor)
            .ThenInclude(v => v.Vendor)
            .Where(q => q.RfqId == rfqId)
            .ToListAsync();

        if (!quotations.Any())
        {
            return new QuotationListResponseDto
            {
                Summary = new QuotationSummaryDto
                {
                    Total = 0,
                    Lowest = 0,
                    Highest = 0,
                    Average = 0
                },
                Quotations = new List<QuotationDto>()
            };
        }

        var summary = new QuotationSummaryDto
        {
            Total = quotations.Count,
            Lowest = quotations.Min(q => q.TotalAmount),
            Highest = quotations.Max(q => q.TotalAmount),
            Average = quotations.Average(q => q.TotalAmount)
        };

        var quotationDtos = quotations.Select(q => new QuotationDto
        {
            Id = q.Id,
            VendorName = q.RfqVendor.Vendor.VendorName,
            VendorEmail = q.RfqVendor.Vendor.Email,
            ContactPerson = q.RfqVendor.Vendor.VendorName,
            SubmittedAt = q.SubmittedAt,
            DeliveryDate = q.DeliveryDate,
            TotalAmount = q.TotalAmount,
            IsSelected = q.IsSelected
        }).ToList();

        return new QuotationListResponseDto
        {
            Summary = summary,
            Quotations = quotationDtos
        };
    }

    public async Task<QuotationDetailsDto> GetQuotationByIdAsync(Guid quotationId)
    {
        var quot = await _context.Quotations
            .Include(i => i.Items)
            .Include(v => v.RfqVendor)
            .ThenInclude(v => v.Vendor)
            .FirstOrDefaultAsync(x => x.Id == quotationId);
        return _mapper.Map<QuotationDetailsDto>(quot);

    }
    
    public async Task<QuotationComparisonResponseDto> CompareQuotationsAsync(List<Guid> quotationIds)
    {
        if (quotationIds == null || !quotationIds.Any())
            throw new ArgumentException("No quotation IDs provided");

        var quotations = await _context.Quotations
            .Include(q => q.Items)
            .Include(q => q.RfqVendor)
            .ThenInclude(v => v.Vendor)
            .Where(q => quotationIds.Contains(q.Id))
            .ToListAsync();

        if (!quotations.Any())
            throw new Exception("No quotations found");

        var quotationDtos = quotations.Select(q => new QuotationDetailResponseDto
        {
            Id = q.Id,
            VendorName = q.RfqVendor.Vendor.CompanyName,
            VendorEmail = q.RfqVendor.Vendor.Email,
            ContactPerson = q.RfqVendor.Vendor.VendorName,

            SubmittedAt = q.SubmittedAt,
            //ValidUntil = q.ValidUntil,

            //SubTotal = q.SubTotal,
            
            TotalAmount = q.TotalAmount,

            //Status = q.Status.ToString(),

            PaymentTerms = q.RfqVendor.Vendor.PaymentTerms.ToString(),
            DeliveryTime = q.DeliveryDate,
            Notes = q.Notes,

            Items = q.Items.Select(i => new QuotationItemDto
            {
                Id = i.Id,
                ItemName = i.ItemName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                VatAmount = i.TaxPercentage,
                Total = i.LineTotal
            }).ToList()
        }).ToList();

        var totals = quotationDtos.Select(q => q.TotalAmount).ToList();

        var lowest = totals.Min();
        var highest = totals.Max();
        var average = totals.Average();
        var priceRange = highest - lowest;

        return new QuotationComparisonResponseDto
        {
            Summary = new ComparisonSummaryDto
            {
                Lowest = lowest,
                Highest = highest,
                Average = average,
                PriceRange = priceRange
            },
            Quotations = quotationDtos
        };
    }
    
    public async Task SelectQuotationAsync(Guid rfqId, Guid quotationId)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            var rfq = await _context.RequestForQuotations
                .Include(r => r.Quotations)
                .FirstOrDefaultAsync(r => r.Id == rfqId);

            if (rfq == null)
                throw new Exception("RFQ not found");

            //if (rfq.Status != RfqStatus.UnderReview)
                //throw new Exception("RFQ is not eligible for selection");

            var quotation = rfq.Quotations
                .FirstOrDefault(q => q.Id == quotationId);

            if (quotation == null)
                throw new Exception("Quotation not found");

            // Reset all selections
            foreach (var q in rfq.Quotations)
                q.IsSelected = false;

            quotation.IsSelected = true;

            rfq.Status = RfqStatus.PendingApproval;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        // ----------------------------------------
        // ENQUEUE APPROVAL CREATION AFTER COMMIT
        // ----------------------------------------

        _backgroundJobClient.Enqueue<IRfqApprovalJob>(
            job => job.SubmitSelectedQuotationForApprovalAsync(rfqId));
    }
    
    
    public async Task ClearSelectedQuotationAsync(Guid rfqId)
    {
        var rfq = await _context.RequestForQuotations
            .Include(r => r.Quotations)
            .FirstOrDefaultAsync(r => r.Id == rfqId);
    
        foreach (var q in rfq.Quotations)
            q.IsSelected = false;
    
        rfq.Status = RfqStatus.UnderReview;
    
        await _context.SaveChangesAsync();
    }
    
    
    public async Task SubmitSelectedQuotationForApproval(Guid rfqId)
    {
        var existing = await _context.Approvals
            .AnyAsync(a =>
                a.ReferenceId == rfqId &&
                a.ReferenceType == ApprovalReferenceType.RFQ);

        if (existing)
            return; // Already submitted

        var rfq = await _context.RequestForQuotations
            .Include(r => r.Quotations)
            .FirstAsync(r => r.Id == rfqId);

        var selected = rfq.Quotations
            .FirstOrDefault(q => q.IsSelected);

        if (selected == null)
            throw new Exception("No selected quotation");

        var requisition = await _context.Requisitions
            .FirstAsync(r => r.Id == rfq.RequisitionId);

        var policies = await _context.ApprovalPolicies
            .Where(p =>
                p.CategoryId == requisition.CategoryId &&
                p.IsActive)
            .OrderBy(p => p.SequenceOrder)
            .ToListAsync();

        var approvals = policies.Select(p => new Approval
        {
            Id = Guid.NewGuid(),
            ReferenceId = rfq.Id,
            ReferenceType = ApprovalReferenceType.RFQ,
            RoleId = p.RoleId,
            SequenceOrder = p.SequenceOrder,
            AssignedAt = DateTime.UtcNow,
            Status = "Pending",
            IsActive = p.SequenceOrder == 1
        }).ToList();

        _context.Approvals.AddRange(approvals);

        await _context.SaveChangesAsync();
        
        BackgroundJob.Enqueue<IEmailJobService>(job => job.SendQuotationApprovalEmailAsync(rfqId));
    }




    private async Task<RfqAccessToken> ValidateTokenAsync(string token)
    {
        var access = await _context.RfqAccessTokens
            .Include(r => r.Rfq)
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                !x.IsUsed &&
                !x.IsExpired &&
                x.ExpiresAt > DateTime.UtcNow);

        if (access == null)
            throw new InvalidOperationException("Invalid or expired RFQ token.");

        if (access.Rfq.Status != RfqStatus.Open)
            throw new InvalidOperationException("RFQ is already closed.");

        return access;
    }
}

public static class SecureTokenGenerator
{
    public static string Generate(int length = 48)
    {
        var bytes = RandomNumberGenerator.GetBytes(length);
        return WebEncoders.Base64UrlEncode(bytes);
    }
}