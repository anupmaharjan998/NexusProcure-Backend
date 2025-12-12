using NexusProcure.Core.DTOs;

namespace NexusProcure.Application.Interfaces;

public interface ICommonService
{
    Task<List<CategoryResponse>> GetAllCategoryAsync();
    Task<CategoryResponse> AddCategoryAsync(CategoryRequest request);
}