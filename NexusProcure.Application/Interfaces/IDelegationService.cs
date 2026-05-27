using NexusProcure.Core.DTOs.Delegation;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces;

public interface IDelegationService
{
    Task<DelegationDto> CreateAsync(string currentUserClaim, CreateDelegationDto dto);

    Task<List<DelegationDto>> GetVisibleDelegationsAsync(string currentUserClaim);

    Task<List<DelegationDto>> GetMyDelegationsAsync(string currentUserClaim);

    Task<User?> GetActiveDelegateAsync(Guid userId);

    Task<bool> DeactivateAsync(string currentUserClaim, Guid delegationId);

    Task<DelegationPermissionsDto> GetPermissionsAsync(string currentUserClaim);

    Task ExpireDelegationsAsync();
}