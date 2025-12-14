using NexusProcure.Core.DTOs;

namespace NexusProcure.Application.Interfaces;

public interface ICommonService
{
    Task<List<CategoryResponse>> GetAllCategoryAsync();
    Task<CategoryResponse?> GetByCategoryByIdAsync(Guid id);
    Task<CategoryResponse> AddCategoryAsync(CategoryRequest request);
    
    Task<CategoryResponse> UpdateCategoryAsync(Guid id, CategoryRequest dto);
    Task<bool> DeleteCategoryAsync(Guid id);
}