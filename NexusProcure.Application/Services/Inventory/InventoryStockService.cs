using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Inventory;

public class InventoryStockService : IInventoryStockService
{
    private readonly NexusProcureDbContext _context;

    public InventoryStockService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task<List<InventoryStockDto>> GetAvailableStockAsync()
    {
        return await _context.InventoryStocks
            .Include(x => x.Category)
            .Where(x => x.QuantityAvailable > 0)
            .OrderBy(x => x.Name)
            .Select(x => new InventoryStockDto
            {
                Id = x.Id,
                Name = x.Name,
                SKU = x.SKU,
                QuantityAvailable = x.QuantityAvailable,
                Unit = x.Unit,
                ReorderLevel = x.ReorderLevel,
                CategoryName = x.Category.Name,
                IsAssetTracked = x.Category.IsAssetTracked
            })
            .ToListAsync();
    }

    public async Task<List<InventoryStockDto>> GetLowStockAsync()
    {
        return await _context.InventoryStocks
            .Include(x => x.Category)
            .Where(x => x.QuantityAvailable <= x.ReorderLevel)
            .OrderBy(x => x.QuantityAvailable)
            .Select(x => new InventoryStockDto
            {
                Id = x.Id,
                Name = x.Name,
                SKU = x.SKU,
                QuantityAvailable = x.QuantityAvailable,
                Unit = x.Unit,
                ReorderLevel = x.ReorderLevel,
                CategoryName = x.Category.Name,
                IsAssetTracked = x.Category.IsAssetTracked
            })
            .ToListAsync();
    }
}