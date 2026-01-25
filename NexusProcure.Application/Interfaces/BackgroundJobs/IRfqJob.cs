namespace NexusProcure.Application.Interfaces.BackgroundJobs;

public interface IRfqJob
{
    Task CreateAndSendRfqAsync(Guid requisitionId);
    Task ValidateTokenAsync();
}