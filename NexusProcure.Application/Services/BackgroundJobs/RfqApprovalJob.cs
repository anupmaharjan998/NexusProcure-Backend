using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.RequestForQuotation;

namespace NexusProcure.Application.Services.BackgroundJobs;

public class RfqApprovalJob : IRfqApprovalJob
{
    private readonly IRfqService _rfqService;

    public RfqApprovalJob(IRfqService rfqService)
    {
        _rfqService = rfqService;
    }

    public async Task SubmitSelectedQuotationForApprovalAsync(Guid rfqId)
    {
        await _rfqService.SubmitSelectedQuotationForApproval(rfqId);
    }
}