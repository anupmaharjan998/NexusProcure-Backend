using NexusProcure.Core.DTOs;

namespace NexusProcure.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardResponseDto> GetDashboardAsync();
}
