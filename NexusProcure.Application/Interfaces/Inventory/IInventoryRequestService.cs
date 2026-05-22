using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Application.Interfaces.Inventory;

public interface IInventoryRequestService
{
    Task<Guid> CreateAsync(Guid userId, CreateInventoryRequestDto dto);
    Task ApproveByManagerAsync(Guid requestId, Guid approverId);
    Task RejectByManagerAsync(Guid requestId, Guid approverId, string? remarks = null);
    Task ProcessByInventoryManagerAsync(Guid requestId, Guid inventoryManagerId, ProcessInventoryRequestDto dto);
    Task<InventoryRequestDto?> GetByIdAsync(Guid requestId);
}