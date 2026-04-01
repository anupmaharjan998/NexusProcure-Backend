using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Common;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Inventory;

public class InventoryService : IInventoryService
{
    private readonly NexusProcureDbContext _context;

    public InventoryService(NexusProcureDbContext context)
    {
        _context = context;
    }

    public async Task ReceiveFromPurchaseOrderAsync(Guid purchaseOrderId)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var po = await _context.PurchaseOrders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == purchaseOrderId);

        if (po == null) throw new Exception("PO not found");

        var grn = new GoodsReceipt
        {
            Id = Guid.NewGuid(),
            PurchaseOrderId = po.Id,
            ReceivedDate = DateTime.UtcNow,
            Items = new List<GoodsReceiptItem>()
        };

        foreach (var item in po.Items)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(x => x.InventoryItemId == item.Id);

            stock.AvailableQuantity += item.Quantity;

            grn.Items.Add(new GoodsReceiptItem
            {
                Id = Guid.NewGuid(),
                InventoryItemId = item.Id,
                QuantityReceived = item.Quantity
            });

            _context.StockTransactions.Add(new StockTransaction
            {
                Id = Guid.NewGuid(),
                InventoryItemId = item.Id,
                Type = StockTransactionType.In,
                Quantity = item.Quantity,
                Date = DateTime.UtcNow,
                Reference = po.Id.ToString()
            });
        }

        await _context.GoodsReceipts.AddAsync(grn);

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }


    public async Task AssignToUserAsync(Guid requisitionId)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var req = await _context.Requisitions
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == requisitionId);

        foreach (var item in req.Items)
        {
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(x => x.InventoryItemId == item.Id);

            if (stock.AvailableQuantity < item.Quantity)
                throw new Exception("Insufficient stock");

            stock.AvailableQuantity -= item.Quantity;

            await _context.InventoryAssignments.AddAsync(new InventoryAssignment
            {
                Id = Guid.NewGuid(),
                InventoryItemId = item.Id,
                UserId = req.RequestedBy.Id,
                Quantity = item.Quantity,
                AssignedDate = DateTime.UtcNow,
                ReferenceId = req.Id
            });

            _context.StockTransactions.Add(new StockTransaction
            {
                Id = Guid.NewGuid(),
                InventoryItemId = item.Id,
                Type = StockTransactionType.Out,
                Quantity = item.Quantity,
                Date = DateTime.UtcNow,
                Reference = req.Id.ToString()
            });
        }

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }

    // public async Task<PaginatedResponse<InventoryItemDto>> GetInventoryAsync(InventoryQueryParams query)
    // {
    //     var items = _context.InventoryItems
    //         .Include(x => x.InventoryCategory)
    //         .Include(x => x.Stock)
    //         .AsQueryable();
    //
    //     // 🔍 SEARCH
    //     if (!string.IsNullOrEmpty(query.Search))
    //     {
    //         items = items.Where(x =>
    //             x.Name.Contains(query.Search) ||
    //             x.SKU.Contains(query.Search));
    //     }
    //
    //     // 📂 CATEGORY
    //     if (query.CategoryId.HasValue)
    //     {
    //         items = items.Where(x => x.InventoryCategoryId == query.CategoryId);
    //     }
    //
    //     // 📊 STATUS FILTER
    //     if (!string.IsNullOrEmpty(query.Status))
    //     {
    //         items = query.Status switch
    //         {
    //             "InStock" => items.Where(x => x.Stock.AvailableQuantity > x.ReorderLevel),
    //             "LowStock" => items.Where(x => x.Stock.AvailableQuantity <= x.ReorderLevel && x.Stock.AvailableQuantity > 0),
    //             "OutOfStock" => items.Where(x => x.Stock.AvailableQuantity == 0),
    //             _ => items
    //         };
    //     }
    //
    //     var total = await items.CountAsync();
    //
    //     var data = await items
    //         .OrderByDescending(x => x.CreatedAt)
    //         .Skip((query.PageNumber - 1) * query.PageSize)
    //         .Take(query.PageSize)
    //         .Select(x => new InventoryItemDto
    //         {
    //             Id = x.Id,
    //             SKU = x.SKU,
    //             Name = x.Name,
    //             Category = x.InventoryCategory.Name,
    //             Quantity = x.Stock.AvailableQuantity,
    //             ReorderLevel = x.ReorderLevel,
    //             //UnitPrice = x.UnitPrice,
    //             Status =
    //                 x.Stock.AvailableQuantity == 0 ? "OutOfStock" :
    //                 x.Stock.AvailableQuantity <= x.ReorderLevel ? "LowStock" :
    //                 "InStock"
    //         })
    //         .ToListAsync();
    //
    //     return new PaginatedResponse<InventoryItemDto>
    //     {
    //         Items = data,
    //         TotalCount = total,
    //         PageNumber = query.PageNumber,
    //         PageSize = query.PageSize
    //     };
    // }
    //
    //
    public async Task<InventoryPagedResponse> GetInventoryAsync(InventoryQueryParams query)
    {
        var itemsQuery = _context.InventoryItems
            .Include(x => x.InventoryCategory)
            .AsQueryable();

        // 🔍 SEARCH
        if (!string.IsNullOrEmpty(query.Search))
        {
            itemsQuery = itemsQuery.Where(x =>
                x.Name.Contains(query.Search) ||
                x.SKU.Contains(query.Search));
        }

        // 📂 CATEGORY
        if (query.CategoryId.HasValue)
        {
            itemsQuery = itemsQuery.Where(x => x.InventoryCategoryId == query.CategoryId);
        }

        // 📊 STATUS FILTER
        if (!string.IsNullOrEmpty(query.Status))
        {
            itemsQuery = query.Status switch
            {
                "Assigned" => itemsQuery.Where(x => x.Status == "Assigned"),
                "Available" => itemsQuery.Where(x => x.Status == "Available" ),
                "Maintenance" => itemsQuery.Where(x => x.Status == "Maintenance" ),
                _ => itemsQuery
            };
        }

        // 🔢 TOTAL COUNT
        var total = await itemsQuery.CountAsync();

        // 📄 PAGINATED DATA
        var data = await itemsQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new InventoryItemDto
            {
                Id = x.Id,
                SKU = x.SKU,
                Name = x.Name,
                Category = x.InventoryCategory.Name,

                Status = x.Status
            })
            .ToListAsync();

        // 📊 KPI STATS (SEPARATE SAFE QUERIES)
        var totalItems = await _context.InventoryItems.CountAsync();

        var assigned = await _context.InventoryItems
            .Where(x => x.Status == "Assigned")
            .CountAsync();
        
        var available = await _context.InventoryItems
            .Where(x => x.Status == "Available")
            .CountAsync();
        var maintenance = await _context.InventoryItems
            .Where(x => x.Status == "Maintenance")
            .CountAsync();


        return new InventoryPagedResponse
        {
            Items = data,
            TotalCount = total,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,

            Stats = new InventoryStatsDto
            {
                TotalItems = totalItems,
                Assigned = assigned,
                Available = available,
                Maintenance = maintenance
            }
        };
    }


    #region Category

    public async Task<CategoryPagedResponse> GetCategoriesAsync(CategoryQueryParams query)
{
    // 🔹 BASE QUERY (ROOT ONLY)
    var baseQuery = _context.InventoryCategories
        .Where(c => c.ParentCategoryId == null)
        .AsQueryable();

    // 🔍 SEARCH (including subcategories)
    if (!string.IsNullOrWhiteSpace(query.Search))
    {
        var search = query.Search.ToLower();

        baseQuery = baseQuery.Where(c =>
            c.Name.ToLower().Contains(search) ||
            c.CategoryCode.ToLower().Contains(search) ||

            _context.InventoryCategories.Any(sc =>
                sc.ParentCategoryId == c.Id &&
                (
                    sc.Name.ToLower().Contains(search) ||
                    sc.CategoryCode.ToLower().Contains(search)
                )
            )
        );
    }

    // ✅ TOTAL COUNT (after filter)
    var totalCount = await baseQuery.CountAsync();

    // ✅ PAGINATED DATA (NO INCLUDE → PURE PROJECTION)
    var categories = await baseQuery
        .OrderByDescending(c => c.CreatedAt)
        .Skip((query.PageNumber - 1) * query.PageSize)
        .Take(query.PageSize)
        .Select(c => new InventoryCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            CategoryCode = c.CategoryCode,
            RiskWeight = c.RiskWeight,
            Description = c.Description,

            // ✅ TOTAL ITEMS (ROOT + SUB)
            TotalItems =
                _context.InventoryItems.Count(i => i.InventoryCategoryId == c.Id) +
                _context.InventoryItems.Count(i =>
                    i.InventoryCategory.ParentCategoryId == c.Id
                ),

            // ✅ SUBCATEGORIES
            SubCategories = _context.InventoryCategories
                .Where(sc => sc.ParentCategoryId == c.Id)
                .Select(sc => new InventoryCategoryDto
                {
                    Id = sc.Id,
                    Name = sc.Name,
                    CategoryCode = sc.CategoryCode,
                    Description = sc.Description,

                    TotalItems = _context.InventoryItems
                        .Count(i => i.InventoryCategoryId == sc.Id),

                    SubCategories = new List<InventoryCategoryDto>()
                }).ToList()
        })
        .ToListAsync();

    // =========================
    // ✅ GLOBAL STATS
    // =========================

    var totalCategories = await _context.InventoryCategories
        .CountAsync(c => c.ParentCategoryId == null);

    var totalSubcategories = await _context.InventoryCategories
        .CountAsync(c => c.ParentCategoryId != null);

    var totalItems = await _context.InventoryItems
        .CountAsync();

    // =========================

    return new CategoryPagedResponse
    {
        Categories = categories,

        CategoryStats = new CategoryStats
        {
            TotalCategories = totalCategories,
            TotalSubCategories = totalSubcategories,
            TotalItems = totalItems
        },

        TotalCount = totalCount,
        PageNumber = query.PageNumber,
        PageSize = query.PageSize
    };
}


    public async Task CreateCategoryAsync(CreateCategoryDto dto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new Exception("Name is required");

        string categoryCode;

        // ✅ ROOT CATEGORY
        if (dto.ParentCategoryId == null)
        {
            categoryCode = await GenerateUniqueRootCode(dto.Name);
        }
        else
        {
            // ✅ SUBCATEGORY
            var parent = await _context.InventoryCategories
                .FirstOrDefaultAsync(x => x.Id == dto.ParentCategoryId);

            if (parent == null)
                throw new Exception("Parent category not found");

            var parentCode = parent.CategoryCode;

            var lastChild = await _context.InventoryCategories
                .Where(x => x.ParentCategoryId == dto.ParentCategoryId)
                .OrderByDescending(x => x.CategoryCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastChild != null)
            {
                var parts = lastChild.CategoryCode.Split('-');
                var lastNumber = parts.Last();

                if (int.TryParse(lastNumber, out int num))
                    nextNumber = num + 1;
            }

            categoryCode = $"{parentCode}-{nextNumber:D3}";
        }

        var category = new InventoryCategory
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            RiskWeight = dto.RiskWeight,
            CategoryCode = categoryCode,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId,
            CreatedBy =  userId
        };

        _context.InventoryCategories.Add(category);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            throw new Exception("Category code conflict. Try again.");
        }
    }

    public async Task UpdateCategoryAsync(Guid id, UpdateInventoryCategoryDto dto, Guid userId)
    {
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category == null)
            throw new Exception("Category not found");

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = userId;

        // Only update code if root category
        if (category.ParentCategoryId == null && !string.IsNullOrEmpty(dto.CategoryCode))
        {
            var exists = await _context.InventoryCategories
                .AnyAsync(x => x.CategoryCode == dto.CategoryCode && x.Id != id);

            if (exists)
                throw new Exception("Category code must be unique");

            category.CategoryCode = dto.CategoryCode;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _context.InventoryCategories
            .Include(c => c.SubCategories)
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            throw new Exception("Category not found");

        if (category.SubCategories.Any())
            throw new Exception("Cannot delete category with subcategories");

        if (category.Items.Any())
            throw new Exception("Cannot delete category with items");

        _context.InventoryCategories.Remove(category);
        await _context.SaveChangesAsync();
    }
    

    #endregion


    #region Inventory Items
    
    public async Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemDto dto, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new Exception("Item name is required");

        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);

        if (category == null)
            throw new Exception("Invalid category");

        // 🔹 Generate SKU
        var sku = await GenerateSkuAsync(dto.Name, category.Id);

        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            InventoryCategoryId = dto.CategoryId,
            SKU = sku,
            Barcode = sku, // ✅ tie barcode to SKU
            Description = dto.Description,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow,
            Status = "Available",
            Condition =  "Good",
            Location = "Inventory"
        };

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        return new InventoryItemDto
        {
            Id = item.Id,
            Name = item.Name,
            SKU = item.SKU,
            //item.Barcode
        };
    }
    
    public async Task<InventoryItemDetailDto> GetInventoryItemById(Guid id)
    {
        var item = await _context.InventoryItems
            .Include(x => x.InventoryCategory)
            .Include(x => x.AssignedTo)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
            throw new Exception("Item not found");

        return new InventoryItemDetailDto
        {
            Id = item.Id,
            Name = item.Name,
            SKU = item.SKU,
            Barcode = item.Barcode,
            SerialNumber = item.SerialNumber,
            Category = item.InventoryCategory.Name,

            Status = item.Status,
            Condition = item.Condition,
            Location = item.Location,

            AssignedTo = item.AssignedTo != null
                ? item.AssignedTo.FullName
                : null,

            AssignedDate = item.AssignedDate,

            Description = item.Description,
            CreatedAt = item.CreatedAt
        };
    }
    
    
    public async Task<List<InventoryCategoryDto>> GetLeafCategories()
    {
        return await _context.InventoryCategories
            .Where(c => !c.SubCategories.Any())
            .Select(c => new InventoryCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                CategoryCode = c.CategoryCode
            })
            .ToListAsync();
    }
    
    
    public async Task<string> GenerateSkuAsync(string name, Guid categoryId)
    {
        var category = await _context.InventoryCategories
            .FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new Exception("Category not found");

        var categoryCode = category.CategoryCode; // e.g., FURN

        var nameCode = new string(name
                .ToUpper()
                .Where(char.IsLetterOrDigit)
                .ToArray())
            .Substring(0, Math.Min(3, name.Length));

        // 🔥 Get last SKU number
        var lastSku = await _context.InventoryItems
            .Where(i => i.SKU.StartsWith($"{categoryCode}-{nameCode}"))
            .OrderByDescending(i => i.SKU)
            .Select(i => i.SKU)
            .FirstOrDefaultAsync();

        int nextNumber = 1;

        if (!string.IsNullOrEmpty(lastSku))
        {
            var parts = lastSku.Split('-');
            if (int.TryParse(parts.Last(), out int num))
            {
                nextNumber = num + 1;
            }
        }

        return $"{categoryCode}-{nameCode}-{nextNumber:D3}";
    }

    #endregion


    public async Task AssignManualAsync(AssignItemDto dto)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var stock = await _context.Stocks
            .FirstOrDefaultAsync(x => x.InventoryItemId == dto.InventoryItemId);

        if (stock.AvailableQuantity < dto.Quantity)
            throw new Exception("Insufficient stock");

        // Deduct
        stock.AvailableQuantity -= dto.Quantity;

        // Assign
        await _context.InventoryAssignments.AddAsync(new InventoryAssignment
        {
            Id = Guid.NewGuid(),
            InventoryItemId = dto.InventoryItemId,
            UserId = dto.UserId,
            Quantity = dto.Quantity,
            AssignedDate = DateTime.UtcNow
        });

        // Ledger
        _context.StockTransactions.Add(new StockTransaction
        {
            Id = Guid.NewGuid(),
            InventoryItemId = dto.InventoryItemId,
            Type = StockTransactionType.Out,
            Quantity = dto.Quantity,
            Date = DateTime.UtcNow,
            Reference = dto.UserId.ToString()
        });

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }


    #region Helper

    private string GenerateCodeFromName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new Exception("Invalid name");

        var words = name
            .Trim()
            .ToUpper()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // Take first 4 characters total
        string code = string.Join("", words.Select(w => w[0]));

        if (code.Length < 4)
        {
            code += string.Join("", words)
                .Substring(0, Math.Min(4 - code.Length, words.Sum(w => w.Length)));
        }

        return code.Length > 4 ? code.Substring(0, 4) : code;
    }


    private async Task<string> GenerateUniqueRootCode(string name)
    {
        var baseCode = GenerateCodeFromName(name);

        var existingCodes = await _context.InventoryCategories
            .Where(x => x.CategoryCode.StartsWith(baseCode))
            .Select(x => x.CategoryCode)
            .ToListAsync();

        if (!existingCodes.Contains(baseCode))
            return baseCode;

        int counter = 1;

        while (true)
        {
            var newCode = $"{baseCode}{counter}";

            if (!existingCodes.Contains(newCode))
                return newCode;

            counter++;
        }
    }
    
    
    

    #endregion
}