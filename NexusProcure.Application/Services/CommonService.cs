using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services;

public class CommonService : ICommonService
{
    private readonly NexusProcureDbContext _context;
    private readonly IMapper _mapper;

    public CommonService(NexusProcureDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CategoryResponse>> GetAllCategoryAsync()
    {
        return await _context.Categories.Select(v => _mapper.Map<CategoryResponse>(v)).ToListAsync();
    }
    
    public async Task<CategoryResponse?> GetByCategoryByIdAsync(Guid id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(r => r.Id == id);

        return category == null ? null : _mapper.Map<CategoryResponse>(category);
    }
    
    public async Task<CategoryResponse> AddCategoryAsync(CategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return _mapper.Map<CategoryResponse>(category);
    }
    
    
    public async Task<CategoryResponse> UpdateCategoryAsync(Guid id, CategoryRequest dto)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return null;
        
        if (!string.IsNullOrEmpty(dto.Name)) category.Name = dto.Name;
        category.Description = dto.Description;
        category.Type = dto.Type;
        
        await _context.SaveChangesAsync();

        return _mapper.Map<CategoryResponse>(category);
    }

    public async Task<bool> DeleteCategoryAsync(Guid id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
}