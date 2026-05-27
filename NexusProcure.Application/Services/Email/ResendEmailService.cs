using Microsoft.Extensions.Configuration;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs.Email;
using Resend;

namespace NexusProcure.Application.Services.Email;

public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly IConfiguration _configuration;

    public ResendEmailService(IResend resend, IConfiguration configuration)
    {
        _resend = resend;
        _configuration = configuration;
    }

    public async Task SendAsync(SendEmailDto dto)
    {
        var fromEmail = _configuration["Resend:FromEmail"];

        if (string.IsNullOrWhiteSpace(fromEmail))
            throw new InvalidOperationException("Resend FromEmail is not configured.");

        if (string.IsNullOrWhiteSpace(dto.To))
            throw new ArgumentException("Recipient email is required.", nameof(dto.To));

        if (string.IsNullOrWhiteSpace(dto.Subject))
            throw new ArgumentException("Email subject is required.", nameof(dto.Subject));

        var message = new EmailMessage
        {
            From = fromEmail,
            Subject = dto.Subject,
            HtmlBody = dto.HtmlBody
        };

        message.To.Add(dto.To);

        await _resend.EmailSendAsync(message);
    }
}