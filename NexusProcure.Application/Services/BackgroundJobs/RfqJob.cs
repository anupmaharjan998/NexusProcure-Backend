using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.RequestForQuotation;
using NexusProcure.Core.DTOs.Email;
using NexusProcure.Core.Entities.RequestForQuotations;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.BackgroundJobs;

public class RfqJob : IRfqJob
{
    private readonly NexusProcureDbContext _context;
    private readonly IRfqService _rfqService;
    private readonly IEmailService _emailService;
    private readonly IConfiguration _config;

    public RfqJob(NexusProcureDbContext context, IRfqService rfqService, IEmailService emailService, IConfiguration config)
    {
        _context = context;
        _rfqService = rfqService;
        _emailService = emailService;
        _config = config;
    }


    public async Task CreateAndSendRfqAsync(Guid requisitionId)
    {
        // 1️⃣ Create RFQ + link vendors
        var rfqCreateRes = await _rfqService.CreateRfqAsync(requisitionId);
        
        var vendorsToken = await _context.RfqAccessTokens
            .Include(v => v.Vendor)
            .Where(t => t.RfqId == rfqCreateRes.Id).ToListAsync();
        // 2️⃣ Generate token + send email to each vendor
        foreach (var vendor in vendorsToken)
        {

            var rfqLink =
                $"{_config["Frontend:Url"]!}/rfq/{vendor.Token}";

            await _emailService.SendAsync(new SendEmailDto
            {
                To = vendor.Vendor.Email,
                Subject = $"RFQ Invitation – {rfqCreateRes.RfqNumber}",
                HtmlBody = $@"
                    <p>Dear {vendor.Vendor.VendorName},</p>

                    <p>You are invited to submit a quotation for RFQ 
                       <strong>{rfqCreateRes.RfqNumber}</strong>.</p>

                    <p>
                        👉 <a href='{rfqLink}'>
                        Open RFQ & Submit Quotation
                        </a>
                    </p>

                    <p>
                    👉 <a href=""{{{{rfqLink}}}}"">Fill quotation online</a><br/>
                    👉 <a href=""{{{{rfqLink}}}}/template"">Download Excel template</a>
                    </p>


                    <p>
                        You may either:
                        <ul>
                            <li>Fill quotation online</li>
                            <li>Upload Excel quotation template</li>
                        </ul>
                    </p>

                    <p>
                        Deadline: {rfqCreateRes.SubmissionDeadline:yyyy-MM-dd}
                    </p>"
            });
            
            _context.RfqAudits.Add(new RfqAudit
            {
                Id = Guid.NewGuid(),
                RfqId = rfqCreateRes.Id,
                Action = "VendorInvited",
                CreatedAt = DateTime.UtcNow,
                PerformedBy = vendor.Vendor.VendorName
            });

        }
    }
    
    public async Task ValidateTokenAsync()
    {
        var now = DateTime.UtcNow;

        var rfqsToClose = await _context.RequestForQuotations
            .Where(r =>
                r.Status == RfqStatus.Open &&
                r.SubmissionDeadline < now)
            .ToListAsync();

        foreach (var rfq in rfqsToClose)
        {
            rfq.Status = RfqStatus.Closed;

            var tokens = await _context.RfqAccessTokens
                .Where(t =>
                    t.RfqId == rfq.Id &&
                    !t.IsUsed &&
                    !t.IsExpired)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsExpired = true;
            }

            _context.RfqAudits.Add(new RfqAudit
            {
                Id = Guid.NewGuid(),
                RfqId = rfq.Id,
                Action = "RFQClosed",
                CreatedAt = now,
                PerformedBy = "System"
            });
        }

        await _context.SaveChangesAsync();
    }
}