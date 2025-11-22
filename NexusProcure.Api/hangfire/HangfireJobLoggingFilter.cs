using Hangfire.Server;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;

namespace NexusProcure.Api.hangfire;

public class HangfireJobLoggingFilter : JobFilterAttribute, IServerFilter, IApplyStateFilter
{
    private readonly ILogger<HangfireJobLoggingFilter> _logger;

    public HangfireJobLoggingFilter(ILogger<HangfireJobLoggingFilter> logger)
    {
        _logger = logger;
    }

    public void OnPerforming(PerformingContext filterContext)
    {
        _logger.LogInformation("Starting job {JobId} - {JobName}",
            filterContext.BackgroundJob.Id,
            filterContext.BackgroundJob.Job.Type.Name);
    }

    public void OnPerformed(PerformedContext filterContext)
    {
        _logger.LogInformation("Finished job {JobId}", filterContext.BackgroundJob.Id);
    }

    public void OnStateApplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        // Log failures
        if (context.NewState is FailedState failedState)
        {
            _logger.LogError(
                failedState.Exception,
                "Hangfire job {JobId} failed: {Reason}",
                context.BackgroundJob.Id,
                failedState.Reason
            );
        }
    }

    public void OnStateUnapplied(ApplyStateContext context, IWriteOnlyTransaction transaction)
    {
        // do nothing
    }
}
