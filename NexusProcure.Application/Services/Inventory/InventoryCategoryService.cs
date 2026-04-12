using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Inventory;

public class InventoryCategoryService : IInventoryCategoryService
{
    private readonly NexusProcureDbContext _context;

    public InventoryCategoryService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(CreateCategoryDto dto)
    {
        // var exists = await _context.InventoryCategories
        //     .AnyAsync(x => x.CategoryCode == dto.CategoryCode);

        //if (exists) throw new Exception("Category code must be unique");

        var category = new InventoryCategory
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            //CategoryCode = dto.CategoryCode,
            ParentCategoryId = dto.ParentCategoryId
        };

        await _context.InventoryCategories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task<List<InventoryCategory>> GetTreeAsync()
    {
        var all = await _context.InventoryCategories.ToListAsync();

        return all.Where(x => x.ParentCategoryId == null)
            .Select(x => BuildTree(x, all)).ToList();
    }

    private InventoryCategory BuildTree(InventoryCategory parent, List<InventoryCategory> all)
    {
        parent.SubCategories = all
            .Where(x => x.ParentCategoryId == parent.Id)
            .Select(x => BuildTree(x, all)).ToList();

        return parent;
    }
}