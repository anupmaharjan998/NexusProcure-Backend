namespace NexusProcure.Application.Interfaces.BackgroundJobs;

public interface IRfqApprovalJob
{
    Task SubmitSelectedQuotationForApprovalAsync(Guid rfqId);
}