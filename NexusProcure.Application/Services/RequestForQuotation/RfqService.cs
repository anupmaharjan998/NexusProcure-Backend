using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Helper;
using NexusProcure.Application.Interfaces.RequestForQuotation;
using NexusProcure.Core.DTOs.RFQ;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Entities.RequestForQuotations;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;
using OfficeOpenXml;

namespace NexusProcure.Application.Services.RequestForQuotation;

public class RfqService : IRfqService
{
    private readonly NexusProcureDbContext _context;
    private readonly IRfqNumberGenerator _rfqNumberGenerator;

    public RfqService(NexusProcureDbContext context, IRfqNumberGenerator rfqNumberGenerator)
    {
        _context = context;
        _rfqNumberGenerator = rfqNumberGenerator;
    }


    public async Task<Core.Entities.RequestForQuotations.RequestForQuotation> CreateRfqAsync(Guid requisitionId)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
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


    public async Task SubmitQuotationAsync(string token, QuotationSubmitDto dto)
    {
        var access = await ValidateTokenAsync(token);

        if (access.Rfq.Status != RfqStatus.Open)
            throw new InvalidOperationException("RFQ is not accepting quotations.");

        if (access.Rfq.SubmissionDeadline < DateTime.UtcNow)
            throw new InvalidOperationException("Submission deadline has passed.");

        // ✅ FIX: GET RFQ-VENDOR RELATION
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

        var quotation = new Quotation
        {
            Id = Guid.NewGuid(),
            RfqId = access.RfqId,
            RfqVendorId = rfqVendor.Id, // ✅ CORRECT FK
            SubmittedAt = DateTime.UtcNow
        };

        _context.Quotations.Add(quotation);

        foreach (var item in dto.Items)
        {
            _context.QuotationItems.Add(new QuotationItem
            {
                Id = Guid.NewGuid(),
                QuotationId = quotation.Id,
                ItemName = item.ItemName,
                UnitPrice = item.UnitPrice,
                TaxPercentage = item.TaxPercentage
            });
        }

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


    public async Task SubmitQuotationFromExcelAsync(
        string token,
        IFormFile file)
    {
        var access = await ValidateTokenAsync(token);

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        using var package = new ExcelPackage(stream);

        var ws = package.Workbook.Worksheets[0];
        if (ws == null)
            throw new InvalidOperationException("Invalid Excel file");

        var quotation = new Quotation
        {
            Id = Guid.NewGuid(),
            RfqId = access.RfqId,
            RfqVendorId = access.VendorId,
            SubmittedAt = DateTime.UtcNow
        };

        _context.Quotations.Add(quotation);

        int row = 2;
        while (!string.IsNullOrWhiteSpace(ws.Cells[row, 1].Text))
        {
            _context.QuotationItems.Add(new QuotationItem
            {
                Id = Guid.NewGuid(),
                QuotationId = quotation.Id,
                UnitPrice = decimal.Parse(ws.Cells[row, 3].Text),
                TaxPercentage = string.IsNullOrEmpty(ws.Cells[row, 4].Text)
                    ? 0
                    : decimal.Parse(ws.Cells[row, 4].Text)
            });

            row++;
        }

        // Audit
        _context.RfqAudits.Add(new RfqAudit
        {
            Id = Guid.NewGuid(),
            RfqId = access.RfqId,
            Action = "ExcelQuotationUploaded",
            CreatedAt = DateTime.UtcNow,
            PerformedBy = access.VendorId.ToString()
        });

        access.IsUsed = true;
        await _context.SaveChangesAsync();
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