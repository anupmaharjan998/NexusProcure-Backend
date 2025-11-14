using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Models;

namespace NexusProcure.Application.Services;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public SmtpEmailService(IOptions<EmailSettings> options)
    {
        _settings = options.Value ?? new EmailSettings();
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        // If not configured, silently no-op to avoid crashing in environments without SMTP
        if (string.IsNullOrWhiteSpace(_settings.Host) || string.IsNullOrWhiteSpace(_settings.FromEmail))
        {
            return;
        }

        using var message = new MailMessage();
        message.From = new MailAddress(_settings.FromEmail, _settings.FromName);
        message.To.Add(new MailAddress(toEmail));
        message.Subject = subject;
        message.Body = htmlBody;
        message.IsBodyHtml = true;

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(_settings.Username))
        {
            client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
        }
        else
        {
            client.UseDefaultCredentials = true;
        }

        await client.SendMailAsync(message);
    }
}