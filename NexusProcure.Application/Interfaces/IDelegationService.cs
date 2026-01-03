using NexusProcure.Core.DTOs.Approval;

namespace NexusProcure.Application.Interfaces;

public interface IDelegationService
{
    Task<DelegationResponseDto> CreateAsync(CreateDelegationDto dto);

    Task<List<DelegationResponseDto>> GetActiveDelegationsAsync();

    Task DeactivateAsync(Guid delegationId);

    Task<ApprovalDelegation?> GetValidDelegationAsync(
        Guid fromUserId,
        Guid toUserId,
        Guid? categoryId,
        Guid? approvalLevelId);
}
