using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Models;
using NexusProcure.Core.DTOs.Email;

namespace NexusProcure.Application.Services;

public class SmtpEmailService 
{
    private readonly IConfiguration _configuration;
    private readonly EmailSettings _settings;

    public SmtpEmailService(IOptions<EmailSettings> options, IConfiguration configuration)
    {
        _configuration = configuration;
        _settings = options.Value ?? new EmailSettings();
    }

    public async Task SendAsync(SendEmailDto dto)
    {
        var smtp = _configuration.GetSection("Smtp");

        var client = new SmtpClient(smtp["Host"], int.Parse(smtp["Port"]!))
        {
            Credentials = new NetworkCredential(
                smtp["Username"],
                smtp["Password"]
            ),
            EnableSsl = true,
            UseDefaultCredentials = false
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtp["FromEmail"]!),
            Subject = dto.Subject,
            Body = dto.HtmlBody,
            IsBodyHtml = true
        };

        mail.To.Add(dto.To);

        await client.SendMailAsync(mail);
    }
}