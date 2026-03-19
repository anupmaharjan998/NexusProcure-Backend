using NexusProcure.Core.DTOs.PurchaseOrder;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces.Procurement;

public interface IPurchaseOrderService
{
    Task<PurchaseOrderListResponse> GetAllAsync();
    Task<PurchaseOrderDto> GetByIdAsync(Guid id);
    Task<PurchaseOrder> CreateAsync(Guid referenceId);
    Task<PurchaseOrderDto> UpdateStatusAsync(Guid id, string status);
}