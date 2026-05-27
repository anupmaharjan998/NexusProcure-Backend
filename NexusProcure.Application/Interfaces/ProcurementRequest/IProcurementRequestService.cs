using NexusProcure.Core.DTOs.ProcurementRequest;

namespace NexusProcure.Application.Interfaces.ProcurementRequest;

public interface IProcurementRequestService
{
    Task<Core.Entities.Inventory.ProcurementRequest> CreateFromApprovedInventoryRequestAsync(
        Guid inventoryRequestId,
        Guid approvedByManagerId,
        string? managerRemarks);

    Task<List<ProcurementRequestListDto>> GetAllAsync();

    Task<ProcurementRequestDetailsDto?> GetByIdAsync(Guid id);

    Task<Guid> CreateRequisitionAsync(
        Guid procurementRequestId,
        Guid procurementOfficerId,
        CreateRequisitionFromProcurementRequestDto dto);

    Task RejectAsync(
        Guid procurementRequestId,
        Guid procurementOfficerId,
        string reason);
}