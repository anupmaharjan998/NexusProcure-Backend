using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.Entities;

namespace NexusProcure.Application.Interfaces.Procurement;

public interface IPurchaseOrderService
{
    Task<List<PurchaseOrder>> GetAllAsync();
    Task<PurchaseOrder> GetByIdAsync(Guid id);
    Task<PurchaseOrder> CreateAsync(PurchaseOrderCreateDto dto);
    Task<PurchaseOrder> UpdateStatusAsync(Guid id, string status);
}