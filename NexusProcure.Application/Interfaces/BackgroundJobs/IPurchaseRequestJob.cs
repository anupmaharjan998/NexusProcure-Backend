namespace NexusProcure.Application.Interfaces.BackgroundJobs;

public interface IPurchaseRequestJob
{
    Task RunAsync();
    Task CreatePurchaseRequestAsync(Guid referenceId);
}