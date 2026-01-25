using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Core.DTOs.Email;
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
        var emailDto = new SendEmailDto
        {
            To = email,
            Subject = subject,
            HtmlBody = body
        };

        await _emailService.SendAsync(emailDto);
    }

    public async Task SendUserPasswordResetTokenEmailAsync(string email, string fullName, string resetLink)
    {
        var subject = "Password Reset Request";
        var body = $"<p>Hi {WebUtility.HtmlEncode(fullName)},</p>" +
                   $"<p>Use the following link to reset your password. The link is valid for 1 hour:</p>" +
                   $"<p><a href='{resetLink}'>Reset Password</a></p>" +
                   "<p>Enter this token in the reset password form to change your password.</p>";


        var emailDto = new SendEmailDto
        {
            To = email,
            Subject = subject,
            HtmlBody = body
        };

        await _emailService.SendAsync(emailDto);
    }

    public async Task SendApprovalNotificationAsync(Guid requisitionId)
    {
        var requisition = await _context.Requisitions
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requisitionId);

        if (requisition == null)
            return;

        var firstRoleId = await _context.Approvals
            .Where(a => a.RequisitionId == requisitionId && a.Status == "Pending" && a.IsActive )
            .Select(a => a.RoleId)
            .FirstOrDefaultAsync();

        if (firstRoleId == Guid.Empty)
            return;

        var approvers = await _context.Users
            .Where(u => u.RoleId == firstRoleId && u.IsActive)
            .Select(u => new { u.Email, u.FullName })
            .ToListAsync();

        foreach (var approver in approvers)
        {
            await _emailService.SendAsync(new SendEmailDto
            {
                To = approver.Email,
                Subject = $"Approval Required: {requisition.RequisitionNumber}",
                HtmlBody = $@"
                        <p>Dear {approver.FullName},</p>
                        <p>A requisition requires your approval.</p>

                        <ul>
                            <li><b>Requisition No:</b> {requisition.RequisitionNumber}</li>
                            <li><b>Risk Level:</b> {requisition.RiskLevel}</li>
                            <li><b>Urgent:</b> {(requisition.IsUrgent ? "Yes" : "No")}</li>
                        </ul>

                        <p>Please log in to NexusProcure to review.</p>
                        <br/>
                        <p><b>NexusProcure System</b></p>
                    "
            });
        }
    }

    public async Task SendApprovalStatusEmailAsync(Guid requisitionId)
    {
        var requisition = await _context.Requisitions
            .Include(r => r.RequestedBy)
            .Include(r => r.Approvals)
            .ThenInclude(a => a.ApprovedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == requisitionId);

        if (requisition == null)
            return;

        var createdUser = requisition.RequestedBy;

        // Build remarks HTML
        string remarksHtml = "<p>No remarks available.</p>";

        if (requisition.Approvals?.Any() == true)
        {
            var remarksBuilder = new StringBuilder();
            remarksBuilder.Append("<ul>");

            foreach (var approval in requisition.Approvals)
            {
                if (!string.IsNullOrWhiteSpace(approval.Comments))
                {
                    remarksBuilder.Append(
                        $"<li><b>{approval.ApprovedBy.FullName}:</b> {approval.Comments}</li>");
                }
            }

            remarksBuilder.Append("</ul>");
            remarksHtml = remarksBuilder.ToString();
        }

        await _emailService.SendAsync(new SendEmailDto
        {
            To = createdUser.Email,
            Subject = $"Requisition: {requisition.RequisitionNumber} Status",
            HtmlBody = $@"
            <p>Dear {createdUser.FullName},</p>

            <p>A requisition requested by you has been <b>{requisition.Status}</b>.</p>

            <ul>
                <li><b>Requisition No:</b> {requisition.RequisitionNumber}</li>
                <li><b>Risk Level:</b> {requisition.RiskLevel}</li>
                <li><b>Urgent:</b> {(requisition.IsUrgent ? "Yes" : "No")}</li>
            </ul>

            <p><b>Remarks:</b></p>
            {remarksHtml}

            <p>Please log in to <b>NexusProcure</b> to review the details.</p>

            <br/>
            <p><b>NexusProcure System</b></p>
        "
        });
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

        var emailDto = new SendEmailDto
        {
            To = email,
            Subject = subject,
            HtmlBody = body
        };

        await _emailService.SendAsync(emailDto);
    }
}