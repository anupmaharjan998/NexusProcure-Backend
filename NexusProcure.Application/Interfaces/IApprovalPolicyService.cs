using NexusProcure.Core.DTOs.Approval;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces;

public interface IApprovalPolicyService
{
    Task<List<ApprovalPolicyResponseDto>> GetPoliciesAsync();
    Task CreateAsync(ApprovalPolicyCreateDto dto);
    Task DeleteAsync(Guid id);

    Task<List<ApprovalLevel>> ResolveApprovalFlowAsync(Guid requisitionId);
}
