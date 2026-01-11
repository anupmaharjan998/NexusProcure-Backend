using NexusProcure.Application.Services.Procurement;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces.Procurement;

public interface IRequisitionApprovalService
{
    Task ApproveRequisitionAsync(Guid requisitionId, Guid approverId, string decision, string comments);
    Task<List<ApprovalLevelResponseDto>> GetRequiredLevelsAsync(decimal amount);
    Task<List<ApprovalDto>> GetApprovalsForRequisitionAsync(Guid requisitionId);
    Task<List<RequisitionResponseDto>> GetPendingApprovalsForRoleAsync(Guid userId);
    
    Task ApproveAsync(Guid requisitionId, Guid approverId, string decision, string comments);

}