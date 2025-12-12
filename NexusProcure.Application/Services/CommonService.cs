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

    public async Task<CategoryResponse> AddCategoryAsync(CategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return _mapper.Map<CategoryResponse>(category);
    }
}