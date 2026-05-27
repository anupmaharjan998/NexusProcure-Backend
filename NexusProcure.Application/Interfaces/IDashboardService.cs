using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Dashboard;

namespace NexusProcure.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardAsync(Guid userId);
}
