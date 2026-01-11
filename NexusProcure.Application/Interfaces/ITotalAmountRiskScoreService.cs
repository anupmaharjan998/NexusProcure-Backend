using NexusProcure.Core.DTOs.Approval;

namespace NexusProcure.Application.Interfaces;

public interface ITotalAmountRiskScoreService
{
    Task<List<TotalAmountRiskScoreResponseDto>> GetAllAsync();
    Task<TotalAmountRiskScoreResponseDto> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(TotalAmountRiskScoreDto dto);
    Task UpdateAsync(Guid id, TotalAmountRiskScoreDto dto);
    Task DeleteAsync(Guid id);
}
