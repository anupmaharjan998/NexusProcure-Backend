using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.ProcurementRequest;

namespace NexusProcure.Application.Services.BackgroundJobs;

public class ProcurementRequestJob : IProcurementRequestJob
{
    private readonly IProcurementRequestService _procurementRequestService;

    public ProcurementRequestJob(IProcurementRequestService procurementRequestService)
    {
        _procurementRequestService = procurementRequestService;
    }

    public async Task CreateInventoryRequestAsync(
        Guid inventoryRequestId,
        Guid approvedByManagerId,
        string? managerRemarks)
    {
        await _procurementRequestService.CreateFromApprovedInventoryRequestAsync(inventoryRequestId,
                approvedByManagerId, managerRemarks);
    }
}