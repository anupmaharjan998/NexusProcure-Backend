namespace NexusProcure.Application.Interfaces.BackgroundJobs;

public interface IProcurementRequestJob
{
    Task CreateInventoryRequestAsync(
        Guid inventoryRequestId,
        Guid approvedByManagerId,
        string? managerRemarks);
}