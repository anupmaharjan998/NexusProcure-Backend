using NexusProcure.Core.DTOs.Category;
using NexusProcure.Core.DTOs.Inventory;

namespace NexusProcure.Application.Interfaces.Inventory;

public interface IInventoryService
{
    Task ReceiveFromPurchaseOrderAsync(Guid purchaseOrderId);
    Task AssignToUserAsync(Guid requisitionId);

    Task<InventoryPagedResponse> GetInventoryAsync(InventoryQueryParams query);

    #region Inventory Category

    Task<CategoryPagedResponse> GetCategoriesAsync(CategoryQueryParams query);
    Task CreateCategoryAsync(CreateCategoryDto dto, Guid userId);
    Task UpdateCategoryAsync(Guid id, UpdateInventoryCategoryDto dto, Guid userId);
    Task DeleteCategoryAsync(Guid id);
    Task<IEnumerable<CategoryDto>> GetLeafCategoriesAsync();
    Task<IEnumerable<InventoryItemDropDownDto>> GetItemsByCategoryAsync(Guid categoryId);

    #endregion

    #region Inventory Item

    Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemDto dto, Guid userId);
    Task<InventoryItemDto> UpdateItemAsync(Guid itemId, UpdateInventoryItemDto dto, Guid userId);
    Task<InventoryItemDetailDto> GetInventoryItemById(Guid id);
    Task<List<InventoryCategoryDto>> GetLeafCategories();
    Task<string> GenerateSkuAsync(string name, Guid categoryId);
    
    Task<InventoryItemDetailDto?> AssignItemAsync(Guid itemId, AssignInventoryItemDto dto, Guid assignedBy);
    Task<InventoryItemDetailDto?> UnassignItemAsync(Guid itemId, Guid unassignedBy);

    #endregion
}