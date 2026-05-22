using NexusProcure.Core.DTOs.Delegation;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces;

public interface IDelegationService
{
    Task<DelegationDto> CreateAsync(Guid userId, CreateDelegationDto dto);
    Task<bool> DeactivateAsync(Guid delegationId);
    Task<User?> GetActiveDelegateAsync(Guid userId);
    Task ExpireDelegationsAsync();
}