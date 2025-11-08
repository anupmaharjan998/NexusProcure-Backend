using NexusProcure.Core.DTOs;

namespace NexusProcure.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetStatsAsync();
}
