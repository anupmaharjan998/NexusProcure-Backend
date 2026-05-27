using NexusProcure.Application.Services.Procurement;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.DTOs.RFQ;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Enums;

namespace NexusProcure.Application.Interfaces.Procurement;

public interface IRequisitionApprovalService
{
    //Todo: Check If these 2 are used if not remove
    Task<List<ApprovalLevelResponseDto>> GetRequiredLevelsAsync(decimal amount);
    Task<List<ApprovalDto>> GetApprovalsForRequisitionAsync(Guid requisitionId);
    
    //This is used not above
    
    Task<List<RequisitionResponseDto>> GetPendingApprovalsForRoleAsync(Guid userId);
    
    Task ApproveAsync(Guid referenceId, Guid approverId, string decision, string comments, ApprovalReferenceType referenceType);

    Task<List<QuotationApprovalListResponseDto>> GetPendingQuotationApprovalsForRoleAsync(Guid userId);
    
    Task ApproveQuoatationAsync(Guid quotationId, Guid approverId, string decision, string comments, ApprovalReferenceType referenceType);

}