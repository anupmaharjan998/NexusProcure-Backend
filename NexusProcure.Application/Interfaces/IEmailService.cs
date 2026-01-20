using NexusProcure.Core.DTOs.Email;

namespace NexusProcure.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(SendEmailDto dto);
}