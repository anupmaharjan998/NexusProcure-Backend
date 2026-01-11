using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces;
using System.Net;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.BackgroundJobs;



public class EmailJobService : IEmailJobService
{
    private readonly IEmailService _emailService;
    private readonly NexusProcureDbContext _context;

    public EmailJobService(IEmailService emailService, NexusProcureDbContext context)
    {
        _emailService = emailService;
        _context = context;
    }

    public async Task SendUserCreatedEmailAsync(string email, string fullName, string username, string password)
    {
        var subject = "Your NexusProcure Account Details";
        var body = $"<p>Hi {WebUtility.HtmlEncode(fullName)},</p>" +
                   $"<p>Your account has been created successfully.</p>" +
                   $"<p><strong>Username:</strong> {WebUtility.HtmlEncode(username)}<br/>" +
                   $"<strong>Email:</strong> {WebUtility.HtmlEncode(email)}<br/>" +
                   $"<strong>Temporary Password:</strong> {WebUtility.HtmlEncode(password)}</p>" +
                   "<p>Please sign in and change your password immediately.</p>";

        await _emailService.SendAsync(email, subject, body);
    }
    public async Task SendUserPasswordResetTokenEmailAsync(string email, string fullName, string resetLink)
    {
        var subject = "Password Reset Request";
        var body = $"<p>Hi {WebUtility.HtmlEncode(fullName)},</p>" +
                   $"<p>Use the following link to reset your password. The link is valid for 1 hour:</p>" +
                   $"<p><a href='{resetLink}'>Reset Password</a></p>" +
                   "<p>Enter this token in the reset password form to change your password.</p>";


        await _emailService.SendAsync(email, subject, body);
    }
    
    public async Task SendApprovalNotificationAsync(Guid approvalId)
    {
        var approval = await _context.Approvals
            .Include(a => a.Requisition)
            .ThenInclude(r => r.RequestedBy)
            //.Include(a => a.AssignedToUser)
            .FirstOrDefaultAsync(a => a.Id == approvalId);

        if (approval == null) return;

        // var email = approval.AssignedToUser.Email;
        var email = "mail@mail.com";
        var subject = $"Requisition Approval Required - {approval.Requisition.Id}";
        var body = $"You have a pending requisition to approve.\nAmount: {approval.Requisition.Items.Sum(i => i.EstimatedCost)}";

        await _emailService.SendAsync(email, subject, body);
    }

    public async Task SendEscalationNotificationAsync(Guid approvalId)
    {
        var approval = await _context.Approvals
            .Include(a => a.Requisition)
            .ThenInclude(r => r.RequestedBy)
            //.Include(a => a.AssignedToUser)
            .FirstOrDefaultAsync(a => a.Id == approvalId);

        if (approval == null) return;

        // var email = approval.AssignedToUser.Email;
        var email = "mail@mail.com";
        var subject = $"Requisition Escalated - {approval.Requisition.Id}";
        var body = $"This requisition has been escalated due to delay. Please take action immediately.";

        await _emailService.SendAsync(email, subject, body);
    }
}

