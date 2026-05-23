using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Application.Interfaces.Inventory;

public interface IInventoryStockService
{
    Task<List<InventoryStockDto>> GetAvailableStockAsync();
    Task<List<InventoryStockDto>> GetLowStockAsync();
}