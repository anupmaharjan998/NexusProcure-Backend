using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Application.Interfaces.Inventory;

public interface IInventoryItemService
{
    Task CreateAsync(CreateInventoryItemDto dto);
    Task UpdateAsync(Guid id, CreateInventoryItemDto dto);
    Task DeleteAsync(Guid id);
}