using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Inventory;

public class InventoryItemService : IInventoryItemService
{
    private readonly NexusProcureDbContext _context;

    public InventoryItemService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(CreateInventoryItemDto dto)
    {
        
    }

    public async Task UpdateAsync(Guid id, CreateInventoryItemDto dto)
    {
        
    }

    public async Task DeleteAsync(Guid id)
    {
       
    }
}