using System.Security.Cryptography;
using ClosedXML.Excel;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
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
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IApprovalPolicyService _approvalPolicyService;

    public RfqService(
        NexusProcureDbContext context,
        IRfqNumberGenerator rfqNumberGenerator,
        IBackgroundJobClient backgroundJobClient,
        IApprovalPolicyService approvalPolicyService)
    {
        _context = context;
        _rfqNumberGenerator = rfqNumberGenerator;
        _backgroundJobClient = backgroundJobClient;
        _approvalPolicyService = approvalPolicyService;
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
                .Include(r => r.Items)
                    .ThenInclude(i => i.InventoryStock)
                        .ThenInclude(s => s.Category)
                .FirstOrDefaultAsync(r => r.Id == requisitionId);

            if (requisition == null)
            {
                throw new InvalidOperationException("Requisition not found.");
            }

            if (!requisition.Items.Any())
            {
                throw new InvalidOperationException("Requisition has no items.");
            }

            var categoryIds = requisition.Items
                .Where(i => i.InventoryStock != null)
                .Select(i => i.InventoryStock.CategoryId)
                .Distinct()
                .ToList();

            if (!categoryIds.Any())
            {
                throw new InvalidOperationException("No inventory categories found for this requisition.");
            }

            var vendors = await _context.Vendors
                .Include(v => v.VendorCategories)
                .Where(v =>
                    v.Status == "Active" &&
                    v.VendorCategories.Any(vc => categoryIds.Contains(vc.CategoryId)))
                .ToListAsync();

            if (!vendors.Any())
            {
                throw new InvalidOperationException("No eligible vendors found for the requisition item categories.");
            }

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

                var rfqVendor = new RfqVendor
                {
                    Id = Guid.NewGuid(),
                    RfqId = rfq.Id,
                    VendorId = vendor.Id,
                    AccessToken = token.Token,
                    TokenExpiresAt = token.ExpiresAt
                };

                _context.RfqVendors.Add(rfqVendor);
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

    public async Task<RfqAccessToken> GenerateAccessTokenAsync(Guid rfqId, Guid vendorId)
    {
        var existing = await _context.RfqAccessTokens
            .FirstOrDefaultAsync(x =>
                x.RfqId == rfqId &&
                x.VendorId == vendorId &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);

        if (existing != null)
        {
            return existing;
        }

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

    public async Task<PublicRfqDto?> GetRfqByTokenAsync(string token)
    {
        var access = await _context.RfqAccessTokens
            .Include(x => x.Vendor)
            .Include(x => x.Rfq)
                .ThenInclude(r => r.Requisition)
                    .ThenInclude(req => req.Items)
                        .ThenInclude(i => i.InventoryStock)
                            .ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);

        if (access == null)
        {
            return null;
        }

        _context.RfqAudits.Add(new RfqAudit
        {
            Id = Guid.NewGuid(),
            RfqId = access.RfqId,
            Action = "VendorAccessed",
            CreatedAt = DateTime.UtcNow,
            PerformedBy = access.VendorId.ToString()
        });

        await _context.SaveChangesAsync();

        return new PublicRfqDto
        {
            RfqNumber = access.Rfq.RfqNumber,
            CreatedAt = access.Rfq.CreatedAt,
            SubmissionDeadline = access.Rfq.SubmissionDeadline,
            Vendor = new RfqVendorDto
            {
                VendorId = access.Vendor.Id,
                VendorName = access.Vendor.VendorName,
                CompanyName = access.Vendor.CompanyName,
                Address = access.Vendor.Address,
                Phone = access.Vendor.PhoneNumber,
                Email = access.Vendor.Email,
                PaymentTerms = access.Vendor.PaymentTerms.ToString()
            },
            Items = access.Rfq.Requisition.Items.Select(i => new PublicRfqItemDto
            {
                RfqItemId = i.Id,
                ItemName = i.InventoryStock.Name,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    public async Task<RfqTokenDto> ValidateRfqTokenAsync(string token)
    {
        var access = await _context.RfqAccessTokens
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);

        if (access == null)
        {
            throw new InvalidOperationException("Invalid or expired RFQ token.");
        }

        return new RfqTokenDto
        {
            RfqId = access.RfqId,
            ExpiresAt = access.ExpiresAt,
            IsUsed = access.IsUsed
        };
    }

    public async Task SubmitQuotationAsync(
        string token,
        QuotationSubmitDto dto,
        string? ipAddress)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var access = await ValidateTokenWithRfqAsync(token);

            if (access.Rfq.Status != RfqStatus.Open)
            {
                throw new InvalidOperationException("RFQ is not accepting quotations.");
            }

            if (access.Rfq.SubmissionDeadline < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Submission deadline has passed.");
            }

            var rfqVendor = await _context.RfqVendors
                .FirstOrDefaultAsync(rv =>
                    rv.RfqId == access.RfqId &&
                    rv.VendorId == access.VendorId);

            if (rfqVendor == null)
            {
                throw new InvalidOperationException("Vendor is not invited to this RFQ.");
            }

            var alreadySubmitted = await _context.Quotations.AnyAsync(q =>
                q.RfqId == access.RfqId &&
                q.RfqVendorId == rfqVendor.Id);

            if (alreadySubmitted)
            {
                throw new InvalidOperationException("Quotation already submitted.");
            }

            var rfqItems = await LoadRfqItemsAsync(access.Rfq.RequisitionId);

            if (dto.Items == null || dto.Items.Count != rfqItems.Count)
            {
                throw new InvalidOperationException("RFQ items were tampered.");
            }

            var quotation = new Quotation
            {
                Id = Guid.NewGuid(),
                RfqId = access.RfqId,
                RfqVendorId = rfqVendor.Id,
                SubmittedAt = DateTime.UtcNow,
                Notes = dto.Notes ?? string.Empty,
                SignedBy = dto.Signature,
                IpAddress = ipAddress ?? "Unknown",
                DeliveryDate = dto.DeliveryTime.ToUniversalTime(),
                SubmissionMethod = "Web Interface"
            };

            decimal totalAmount = 0;

            foreach (var submittedItem in dto.Items)
            {
                var originalItem = rfqItems.FirstOrDefault(i => i.Id == submittedItem.RfqItemId);

                if (originalItem == null)
                {
                    throw new InvalidOperationException($"Invalid RFQ item detected: {submittedItem.ItemName}");
                }

                if (originalItem.Quantity != submittedItem.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Quantity tampering detected for item '{originalItem.InventoryStock.Name}'.");
                }

                var lineTotal = CalculateLineTotal(
                    submittedItem.Quantity,
                    submittedItem.UnitPrice,
                    submittedItem.TaxPercentage);

                totalAmount += lineTotal;

                _context.QuotationItems.Add(new QuotationItem
                {
                    Id = Guid.NewGuid(),
                    QuotationId = quotation.Id,
                    ItemName = originalItem.InventoryStock.Name,
                    InventoryCategoryId = originalItem.InventoryStock.Category.Id,
                    Quantity = submittedItem.Quantity,
                    UnitPrice = submittedItem.UnitPrice,
                    TaxPercentage = submittedItem.TaxPercentage
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
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task SubmitQuotationFromExcelAsync(string token, IFormFile file)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        try
        {
            var access = await ValidateTokenWithRfqAsync(token);

            if (access.Rfq.Status != RfqStatus.Open)
            {
                throw new InvalidOperationException("RFQ is not accepting quotations.");
            }

            if (access.Rfq.SubmissionDeadline < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Submission deadline has passed.");
            }

            var rfqVendor = await _context.RfqVendors
                .FirstOrDefaultAsync(rv =>
                    rv.RfqId == access.RfqId &&
                    rv.VendorId == access.VendorId);

            if (rfqVendor == null)
            {
                throw new InvalidOperationException("Vendor is not invited to this RFQ.");
            }

            var alreadySubmitted = await _context.Quotations.AnyAsync(q =>
                q.RfqId == access.RfqId &&
                q.RfqVendorId == rfqVendor.Id);

            if (alreadySubmitted)
            {
                throw new InvalidOperationException("Quotation already submitted.");
            }

            var rfqItems = await LoadRfqItemsAsync(access.Rfq.RequisitionId);

            if (!rfqItems.Any())
            {
                throw new InvalidOperationException("RFQ has no items.");
            }

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);
            var ws = workbook.Worksheet(1);

            var excelItems = new List<(string ItemName, int Quantity, decimal UnitPrice, decimal Vat)>();

            var row = 5;

            while (!ws.Cell(row, 1).IsEmpty())
            {
                excelItems.Add((
                    ItemName: ws.Cell(row, 1).GetString().Trim(),
                    Quantity: ws.Cell(row, 2).GetValue<int>(),
                    UnitPrice: ws.Cell(row, 3).GetValue<decimal>(),
                    Vat: ws.Cell(row, 4).GetValue<decimal>()
                ));

                row++;
            }

            if (excelItems.Count != rfqItems.Count)
            {
                throw new InvalidOperationException("RFQ items have been tampered.");
            }

            var quotation = new Quotation
            {
                Id = Guid.NewGuid(),
                RfqId = access.RfqId,
                RfqVendorId = rfqVendor.Id,
                SubmittedAt = DateTime.UtcNow,
                Notes = "Submitted using Excel upload",
                SubmissionMethod = "Excel Upload",
                DeliveryDate = DateTime.UtcNow
            };

            decimal totalAmount = 0;

            foreach (var excelItem in excelItems)
            {
                var originalItem = rfqItems.FirstOrDefault(i =>
                    NormalizeItemName(i.InventoryStock.Name) == NormalizeItemName(excelItem.ItemName));

                if (originalItem == null)
                {
                    throw new InvalidOperationException($"Invalid item '{excelItem.ItemName}' detected.");
                }

                if (originalItem.Quantity != excelItem.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Quantity tampering detected for item '{originalItem.InventoryStock.Name}'.");
                }

                var lineTotal = CalculateLineTotal(
                    excelItem.Quantity,
                    excelItem.UnitPrice,
                    excelItem.Vat);

                totalAmount += lineTotal;

                _context.QuotationItems.Add(new QuotationItem
                {
                    Id = Guid.NewGuid(),
                    QuotationId = quotation.Id,
                    ItemName = originalItem.InventoryStock.Name,
                    Quantity = excelItem.Quantity,
                    UnitPrice = excelItem.UnitPrice,
                    TaxPercentage = excelItem.Vat
                });
            }

            quotation.TotalAmount = totalAmount;

            _context.Quotations.Add(quotation);

            access.IsUsed = true;

            _context.RfqAudits.Add(new RfqAudit
            {
                Id = Guid.NewGuid(),
                RfqId = access.RfqId,
                Action = "ExcelQuotationUploaded",
                CreatedAt = DateTime.UtcNow,
                PerformedBy = access.VendorId.ToString()
            });

            await _context.SaveChangesAsync();
            await tx.CommitAsync();
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }

    public async Task<List<RfqDto>> GetAllRfqAsync()
    {
        var rfqs = await _context.RequestForQuotations
            .Include(r => r.Requisition)
            .Include(r => r.Quotations)
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return rfqs.Select(r => new RfqDto
        {
            Id = r.Id,
            RfqNumber = r.RfqNumber,
            RequisitionId = r.RequisitionId,
            CreatedAt = r.CreatedAt,
            SubmissionDeadline = r.SubmissionDeadline,
            Status = r.Status,
            QuotationCount = r.Quotations.Count
        }).ToList();
    }

    public async Task<QuotationListResponseDto> GetQuotationByRfqIdAsync(Guid rfqId)
    {
        var quotations = await _context.Quotations
            .Include(q => q.RfqVendor)
                .ThenInclude(rv => rv.Vendor)
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

        return new QuotationListResponseDto
        {
            Summary = new QuotationSummaryDto
            {
                Total = quotations.Count,
                Lowest = quotations.Min(q => q.TotalAmount),
                Highest = quotations.Max(q => q.TotalAmount),
                Average = quotations.Average(q => q.TotalAmount)
            },
            Quotations = quotations.Select(q => new QuotationDto
            {
                Id = q.Id,
                VendorName = q.RfqVendor.Vendor.VendorName,
                VendorEmail = q.RfqVendor.Vendor.Email,
                ContactPerson = q.RfqVendor.Vendor.VendorName,
                SubmittedAt = q.SubmittedAt,
                DeliveryDate = q.DeliveryDate,
                TotalAmount = q.TotalAmount,
                IsSelected = q.IsSelected
            }).ToList()
        };
    }

    public async Task<QuotationDetailsDto> GetQuotationByIdAsync(Guid quotationId)
    {
        var quotation = await _context.Quotations
            .Include(q => q.Items)
            .Include(q => q.RfqVendor)
                .ThenInclude(rv => rv.Vendor)
            .FirstOrDefaultAsync(q => q.Id == quotationId);

        if (quotation == null)
        {
            throw new InvalidOperationException("Quotation not found.");
        }

        return new QuotationDetailsDto
        {
            Id = quotation.Id,
            VendorName = quotation.RfqVendor.Vendor.VendorName,
            VendorEmail = quotation.RfqVendor.Vendor.Email,
            SubmittedAt = quotation.SubmittedAt,
            DeliveryDate = quotation.DeliveryDate,
            TotalAmount = quotation.TotalAmount,
            Notes = quotation.Notes,
            IsSelected = quotation.IsSelected,
            Items = quotation.Items.Select(i => new QuotationItemDto
            {
                Id = i.Id,
                ItemName = i.ItemName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TaxPercentage = i.TaxPercentage,
                LineTotal = CalculateLineTotal(i.Quantity, i.UnitPrice, i.TaxPercentage)
            }).ToList()
        };
    }

    public async Task<QuotationComparisonResponseDto> CompareQuotationsAsync(List<Guid> quotationIds)
    {
        if (quotationIds == null || quotationIds.Count < 2)
        {
            throw new ArgumentException("At least two quotations are required for comparison.");
        }

        var quotations = await _context.Quotations
            .Include(q => q.Items)
            .Include(q => q.RfqVendor)
                .ThenInclude(rv => rv.Vendor)
            .Where(q => quotationIds.Contains(q.Id))
            .ToListAsync();

        if (!quotations.Any())
        {
            throw new InvalidOperationException("No quotations found.");
        }
        
        var totals = quotations.Select(q => q.TotalAmount).ToList();
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
            Quotations = quotations.Select(q => new QuotationDetailResponseDto
            {
                Id = q.Id,
                VendorName = q.RfqVendor.Vendor.CompanyName,
                VendorEmail = q.RfqVendor.Vendor.Email,
                ContactPerson = q.RfqVendor.Vendor.VendorName,
                SubmittedAt = q.SubmittedAt,
                TotalAmount = q.TotalAmount,
                PaymentTerms = q.RfqVendor.Vendor.PaymentTerms.ToString(),
                DeliveryTime = q.DeliveryDate,
                Notes = q.Notes,
                Items = q.Items.Select(i => new QuotationItemDto
                {
                    Id = i.Id,
                    ItemName = i.ItemName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TaxPercentage = i.TaxPercentage,
                    LineTotal = CalculateLineTotal(i.Quantity, i.UnitPrice, i.TaxPercentage)
                }).ToList()
            }).ToList()
        };
    }

    public async Task SelectQuotationAsync(Guid rfqId, Guid quotationId)
    {
        var rfq = await _context.RequestForQuotations
            .Include(r => r.Quotations)
            .FirstOrDefaultAsync(r => r.Id == rfqId);

        if (rfq == null)
        {
            throw new InvalidOperationException("RFQ not found.");
        }

        var selectedQuotation = rfq.Quotations
            .FirstOrDefault(q => q.Id == quotationId);

        if (selectedQuotation == null)
        {
            throw new InvalidOperationException("Quotation does not belong to this RFQ.");
        }

        foreach (var quotation in rfq.Quotations)
        {
            quotation.IsSelected = quotation.Id == quotationId;
        }

        rfq.Status = RfqStatus.PendingApproval;

        await _context.SaveChangesAsync();
        
        _backgroundJobClient.Enqueue<IRfqApprovalJob>(
            job => job.SubmitSelectedQuotationForApprovalAsync(rfqId));
    }

    public async Task ClearSelectedQuotationAsync(Guid rfqId)
    {
        var quotations = await _context.Quotations
            .Where(q => q.RfqId == rfqId)
            .ToListAsync();

        foreach (var quotation in quotations)
        {
            quotation.IsSelected = false;
        }

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
            .FirstOrDefaultAsync(r => r.Id == rfqId);

        if (rfq == null)
        {
            throw new InvalidOperationException("RFQ not found.");
        }

        var selectedQuotation = rfq.Quotations.FirstOrDefault(q => q.IsSelected);

        if (selectedQuotation == null)
        {
            throw new InvalidOperationException("No quotation selected.");
        }

        rfq.Status = RfqStatus.PendingApproval;
        
        var requisition = await _context.Requisitions
            .Include(r => r.Items)
            .ThenInclude(i => i.InventoryStock)
            .FirstAsync(r => r.Id == rfq.RequisitionId);
        
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
                ReferenceType = ApprovalReferenceType.RFQ,
                RoleId = step.RoleId,
                Status = "Pending",
                SequenceOrder = step.SequenceOrder,
                IsActive = step.SequenceOrder == firstSequenceOrder,
                AssignedAt = DateTime.UtcNow,
                Escalated = false
            });
        }

        await _context.SaveChangesAsync();
    }

    private async Task<RfqAccessToken> ValidateTokenWithRfqAsync(string token)
    {
        var access = await _context.RfqAccessTokens
            .Include(x => x.Rfq)
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                !x.IsUsed &&
                x.ExpiresAt > DateTime.UtcNow);

        if (access == null)
        {
            throw new InvalidOperationException("Invalid or expired RFQ token.");
        }

        return access;
    }

    private async Task<List<RequisitionItem>> LoadRfqItemsAsync(Guid requisitionId)
    {
        return await _context.RequisitionItems
            .Include(i => i.InventoryStock)
                .ThenInclude(s => s.Category)
            .Where(i => i.RequisitionId == requisitionId)
            .ToListAsync();
    }

    private static string NormalizeItemName(string value)
    {
        return (value ?? string.Empty).Trim().ToLowerInvariant();
    }

    private static decimal CalculateLineTotal(
        int quantity,
        decimal unitPrice,
        decimal taxPercentage)
    {
        var subtotal = unitPrice * quantity;
        var tax = subtotal * taxPercentage / 100m;

        return subtotal + tax;
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