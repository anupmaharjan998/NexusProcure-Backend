using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces;
using System.Net;

namespace NexusProcure.Application.Services.BackgroundJobs;



public class EmailJobService : IEmailJobService
{
    private readonly IEmailService _emailService;

    public EmailJobService(IEmailService emailService)
    {
        _emailService = emailService;
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
}
