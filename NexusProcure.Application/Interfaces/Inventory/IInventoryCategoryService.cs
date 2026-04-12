using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.Entities.Inventory;

namespace NexusProcure.Application.Interfaces.Inventory;

public interface IInventoryCategoryService
{
    Task CreateAsync(CreateCategoryDto dto);
    Task<List<InventoryCategory>> GetTreeAsync();
}