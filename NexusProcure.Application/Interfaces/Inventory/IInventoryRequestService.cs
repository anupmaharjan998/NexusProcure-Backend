using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Application.Interfaces.Inventory;

public interface IInventoryRequestService
{
    Task<Guid> CreateAsync(Guid userId, CreateInventoryRequestDto dto);

    Task<List<InventoryRequestSummaryDto>> GetMyRequestsAsync(Guid userId);

    Task<List<InventoryRequestSummaryDto>> GetPendingForManagerAsync(Guid managerId);

    Task<List<InventoryRequestSummaryDto>> GetApprovedForInventoryManagerAsync();

    Task<InventoryRequestDto?> GetByIdAsync(Guid requestId);

    Task<List<AvailableInventoryItemDto>> GetAvailableAssetsByStockAsync(Guid stockId);

    Task ApproveByManagerAsync(Guid requestId, Guid approverId);

    Task RejectByManagerAsync(Guid requestId, Guid approverId, string? remarks = null);

    Task ProcessByInventoryManagerAsync(Guid requestId, Guid inventoryManagerId, ProcessInventoryRequestDto dto);
    
    Task<List<InventoryRequestSummaryDto>> GetShortagePendingForManagerAsync(Guid managerId);

    Task SendShortageToProcurementAsync(Guid requestId, Guid managerId, string? remarks = null);

    Task RejectShortageAsync(Guid requestId, Guid managerId, string? remarks = null);
    
    Task<List<MyAssignedInventoryItemDto>> GetMyAssignedItemsAsync(Guid userId);
    Task<MyAssignedInventoryItemDetailDto?> GetMyAssignedItemDetailAsync(Guid userId, Guid itemId);
}