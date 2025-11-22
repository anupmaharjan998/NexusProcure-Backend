namespace NexusProcure.Application.Interfaces.BackgroundJobs;

public interface IEmailJobService
{
    Task SendUserCreatedEmailAsync(string email, string fullName, string username, string password);
}