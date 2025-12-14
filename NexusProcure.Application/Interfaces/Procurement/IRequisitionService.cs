using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces.Procurement;

public interface IRequisitionService
{
    Task<IEnumerable<RequisitionResponseDto>> GetAllAsync();
    Task<RequisitionResponseDto> GetByIdAsync(Guid id);
    Task<RequisitionResponseDto> CreateAsync(RequisitionCreateDto dto);
    Task<RequisitionResponseDto> ApproveAsync(Guid requisitionId, Guid approvedById, string comments);
    Task<RequisitionResponseDto> RejectAsync(Guid requisitionId, Guid rejectedById, string comments);
}