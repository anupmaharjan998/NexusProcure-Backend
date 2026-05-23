using NexusProcure.Core.DTOs.Category;
using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Application.Interfaces.Inventory;

public interface IInventoryService
{
    Task ReceiveFromPurchaseOrderAsync(Guid purchaseOrderId);

    Task<InventoryPagedResponse> GetInventoryAsync(InventoryQueryParams query);

    Task<InventoryStockPagedResponse> GetStocksAsync(InventoryStockQueryParams query);
    Task<InventoryStockDto> CreateStockAsync(CreateInventoryStockDto dto, Guid userId);
    Task<InventoryStockDto> UpdateStockAsync(Guid stockId, UpdateInventoryStockDto dto, Guid userId);
    Task AdjustStockAsync(Guid stockId, AdjustInventoryStockDto dto, Guid userId);
    Task<List<InventoryStockDto>> GetAvailableStocksAsync();

    Task<CategoryPagedResponse> GetCategoriesAsync(CategoryQueryParams query);
    Task CreateCategoryAsync(CreateCategoryDto dto, Guid userId);
    Task UpdateCategoryAsync(Guid id, UpdateInventoryCategoryDto dto, Guid userId);
    Task DeleteCategoryAsync(Guid id);
    Task<IEnumerable<CategoryDto>> GetLeafCategoriesAsync();
    Task<List<InventoryCategoryDto>> GetLeafCategories();

    Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemDto dto, Guid userId);
    Task<InventoryItemDto> UpdateItemAsync(Guid itemId, UpdateInventoryItemDto dto, Guid userId);
    Task<InventoryItemDetailDto> GetInventoryItemById(Guid id);
    Task<string> GenerateSkuAsync(string name, Guid categoryId);
    Task<IEnumerable<InventoryItemDropDownDto>> GetItemsByCategoryAsync(Guid categoryId);
    Task<IEnumerable<InventoryItemDropDownDto>> GetAvailableAssetsByStockAsync(Guid stockId);

    Task<InventoryItemDetailDto?> AssignItemAsync(Guid itemId, AssignInventoryItemDto dto, Guid assignedBy);
    Task<InventoryItemDetailDto?> UnassignItemAsync(Guid itemId, Guid unassignedBy);
}