using NexusProcure.Core.DTOs.Approval;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces;

public interface IApprovalPolicyService
{
    Task<List<ApprovalPolicyResponseDto>> GetPoliciesAsync();
    Task CreateAsync(ApprovalPolicyCreateDto dto);
    Task DeleteAsync(Guid id);

    Task<List<ApprovalPolicy>> ResolveApprovalFlowAsync(Requisition requisition);
    Task<List<ApprovalPolicy>> ResolveApprovalFlowByIdAsync(Guid requisitionId);
}
