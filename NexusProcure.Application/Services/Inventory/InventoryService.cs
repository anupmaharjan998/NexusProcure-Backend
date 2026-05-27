using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Category;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Inventory;

public class InventoryService : IInventoryService
{
    private readonly NexusProcureDbContext _context;

    public InventoryService(NexusProcureDbContext context)
    {
        _context = context;
    }

    // ============================================================
    // LEGACY-COMPATIBLE PO RECEIVE, NOW USING NEW INVENTORY FLOW
    // ============================================================

    public async Task ReceiveFromPurchaseOrderAsync(Guid purchaseOrderId, Guid? receivedBy)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var po = await _context.PurchaseOrders
            .Include(x => x.Items)
            .FirstOrDefaultAsync(x => x.Id == purchaseOrderId);

        if (po == null)
            throw new Exception("PO not found.");

        var grn = new GoodsReceipt
        {
            Id = Guid.NewGuid(),
            PurchaseOrderId = po.Id,
            ReceivedDate = DateTime.UtcNow,
            ReceivedById = receivedBy ?? Guid.Empty,
            InventoryProcessingStatus = InventoryProcessingStatus.Complete,
            Items = new List<GoodsReceiptItem>()
        };

        foreach (var poItem in po.Items)
        {
            if (poItem.InventoryCategoryId == null)
                throw new Exception($"Inventory category missing for {poItem.ItemName}.");

            var category = await _context.InventoryCategories
                .FirstOrDefaultAsync(x => x.Id == poItem.InventoryCategoryId.Value);

            if (category == null)
                throw new Exception($"Category not found for {poItem.ItemName}.");

            var stock = await _context.InventoryStocks
                .FirstOrDefaultAsync(x =>
                    x.Name == poItem.ItemName &&
                    x.CategoryId == category.Id);

            if (stock == null)
            {
                var sku = await GenerateSkuAsync(poItem.ItemName, category.Id);

                stock = new InventoryStock
                {
                    Id = Guid.NewGuid(),
                    Name = poItem.ItemName,
                    SKU = sku,
                    CategoryId = category.Id,
                    QuantityAvailable = 0,
                    Unit = string.IsNullOrWhiteSpace(poItem.Unit) ? "pcs" : poItem.Unit,
                    ReorderLevel = poItem.ReorderLevel <= 0 ? 5 : poItem.ReorderLevel,
                    CreatedById = receivedBy,
                    CreatedAt = DateTime.UtcNow
                };

                _context.InventoryStocks.Add(stock);
                await _context.SaveChangesAsync();
            }

            stock.QuantityAvailable += poItem.Quantity;

            grn.Items.Add(new GoodsReceiptItem
            {
                Id = Guid.NewGuid(),
                PurchaseOrderItemId = poItem.Id,
                QuantityReceived = poItem.Quantity,
                Location = "Inventory",
                Condition = "Good",
                InventoryInserted = true,
                InventoryInsertedAt = DateTime.UtcNow
            });

            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                StockId = stock.Id,
                QuantityChange = poItem.Quantity,
                Type = InventoryTransactionType.Receive,
                ReferenceId = po.Id,
                TransactionDate = DateTime.UtcNow,
                PerformedById = receivedBy,
                Remarks = $"Received from PO {po.PurchaseOrderNumber}"
            });

            if (category.IsAssetTracked)
            {
                for (int i = 0; i < poItem.Quantity; i++)
                {
                    var assetSku = await GenerateSkuAsync(poItem.ItemName, category.Id);

                    var asset = new InventoryItem
                    {
                        Id = Guid.NewGuid(),
                        StockId = stock.Id,
                        Name = poItem.ItemName,
                        Description = string.Empty,
                        SKU = assetSku,
                        Barcode = assetSku,
                        Status = InventoryItemStatus.Available,
                        Condition = InventoryItemCondition.Good,
                        Location = "Inventory",
                        InventoryCategoryId = category.Id,
                        CreatedById = receivedBy,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.InventoryItems.Add(asset);
                }
            }
        }

        _context.GoodsReceipts.Add(grn);
        po.DeliveryStatus = DeliveryStatus.Received;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }

    // ============================================================
    // LEGACY-COMPATIBLE REQUISITION ASSIGN, NOW USING NEW FLOW
    // ============================================================

    // public async Task AssignToUserAsync(Guid requisitionId, Guid assignedBy)
    // {
    //     await using var tx = await _context.Database.BeginTransactionAsync();
    //
    //     var req = await _context.Requisitions
    //         .Include(r => r.Items)
    //         .Include(r => r.RequestedBy)
    //         .FirstOrDefaultAsync(r => r.Id == requisitionId);
    //
    //     if (req == null)
    //         throw new Exception("Requisition not found.");
    //
    //     if (req.RequestedBy == null)
    //         throw new Exception("Requested user not found.");
    //
    //     foreach (var reqItem in req.Items)
    //     {
    //         var stock = await _context.InventoryStocks
    //             .Include(x => x.Category)
    //             .FirstOrDefaultAsync(x => x.Name == reqItem.ItemName);
    //
    //         if (stock == null)
    //             throw new Exception($"Stock not found for {reqItem.ItemName}.");
    //
    //         if (stock.QuantityAvailable < reqItem.Quantity)
    //             throw new Exception($"Insufficient stock for {stock.Name}.");
    //
    //         if (stock.Category.IsAssetTracked)
    //         {
    //             var availableAssets = await _context.InventoryItems
    //                 .Where(x =>
    //                     x.StockId == stock.Id &&
    //                     x.Status == InventoryItemStatus.Available)
    //                 .Take(reqItem.Quantity)
    //                 .ToListAsync();
    //
    //             if (availableAssets.Count < reqItem.Quantity)
    //                 throw new Exception($"Not enough available assets for {stock.Name}.");
    //
    //             foreach (var asset in availableAssets)
    //             {
    //                 asset.Status = InventoryItemStatus.Assigned;
    //                 asset.AssignedToId = req.RequestedBy.Id;
    //                 asset.AssignedDate = DateTime.UtcNow;
    //
    //                 _context.InventoryAssignmentHistories.Add(new InventoryAssignmentHistory
    //                 {
    //                     Id = Guid.NewGuid(),
    //                     InventoryItemId = asset.Id,
    //                     AssignedToId = req.RequestedBy.Id,
    //                     AssignedDate = DateTime.UtcNow,
    //                     ActionType = "ASSIGNED",
    //                     PerformedById = assignedBy,
    //                     Notes = $"Assigned from requisition {req.Id}"
    //                 });
    //             }
    //         }
    //
    //         stock.QuantityAvailable -= reqItem.Quantity;
    //
    //         _context.InventoryTransactions.Add(new InventoryTransaction
    //         {
    //             Id = Guid.NewGuid(),
    //             StockId = stock.Id,
    //             QuantityChange = -reqItem.Quantity,
    //             Type = InventoryTransactionType.Issue,
    //             ReferenceId = req.Id,
    //             TransactionDate = DateTime.UtcNow,
    //             PerformedById = assignedBy,
    //             Remarks = $"Issued from requisition {req.Id}"
    //         });
    //     }
    //
    //     await _context.SaveChangesAsync();
    //     await tx.CommitAsync();
    // }

    // ============================================================
    // STOCK / CATALOG
    // ============================================================

    public async Task<InventoryStockPagedResponse> GetStocksAsync(InventoryStockQueryParams query)
    {
        var stocksQuery = _context.InventoryStocks
            .Include(x => x.Category)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();

            stocksQuery = stocksQuery.Where(x =>
                x.Name.ToLower().Contains(search) ||
                x.SKU.ToLower().Contains(search) ||
                x.Category.Name.ToLower().Contains(search));
        }

        if (query.CategoryId.HasValue)
        {
            stocksQuery = stocksQuery.Where(x => x.CategoryId == query.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            stocksQuery = query.Status switch
            {
                "InStock" => stocksQuery.Where(x => x.QuantityAvailable > x.ReorderLevel),
                "LowStock" => stocksQuery.Where(x => x.QuantityAvailable <= x.ReorderLevel && x.QuantityAvailable > 0),
                "OutOfStock" => stocksQuery.Where(x => x.QuantityAvailable == 0),
                _ => stocksQuery
            };
        }

        var total = await stocksQuery.CountAsync();

        var data = await stocksQuery
            .OrderByDescending(x => x.CreatedAt)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new InventoryStockDto
            {
                Id = x.Id,
                Name = x.Name,
                SKU = x.SKU,
                QuantityAvailable = x.QuantityAvailable,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,
                Unit = x.Unit,
                ReorderLevel = x.ReorderLevel,
                IsAssetTracked = x.Category.IsAssetTracked,
                Status =
                    x.QuantityAvailable == 0 ? "OutOfStock" :
                    x.QuantityAvailable <= x.ReorderLevel ? "LowStock" :
                    "InStock"
            })
            .ToListAsync();

        return new InventoryStockPagedResponse
        {
            Items = data,
            TotalCount = total,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            Stats = new InventoryStockStatsDto
            {
                TotalStocks = await _context.InventoryStocks.CountAsync(),
                LowStock = await _context.InventoryStocks.CountAsync(x => x.QuantityAvailable <= x.ReorderLevel && x.QuantityAvailable > 0),
                OutOfStock = await _context.InventoryStocks.CountAsync(x => x.QuantityAvailable == 0),
                AssetTrackedStocks = await _context.InventoryStocks.CountAsync(x => x.Category.IsAssetTracked)
            }
        };
    }

    public async Task<InventoryStockDto> CreateStockAsync(CreateInventoryStockDto dto, Guid userId)
    {
        var category = await _context.InventoryCategories.FirstOrDefaultAsync(x => x.Id == dto.CategoryId);

        if (category == null)
            throw new Exception("Invalid category.");

        var exists = await _context.InventoryStocks.AnyAsync(x =>
            x.CategoryId == dto.CategoryId &&
            x.Name.ToLower() == dto.Name.Trim().ToLower());

        if (exists)
            throw new Exception("Stock item already exists in this category.");

        var sku = await GenerateSkuAsync(dto.Name, category.Id);

        var stock = new InventoryStock
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            SKU = sku,
            CategoryId = category.Id,
            QuantityAvailable = dto.OpeningQuantity < 0 ? 0 : dto.OpeningQuantity,
            Unit = string.IsNullOrWhiteSpace(dto.Unit) ? "pcs" : dto.Unit.Trim(),
            ReorderLevel = dto.ReorderLevel < 0 ? 0 : dto.ReorderLevel,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.InventoryStocks.Add(stock);

        if (stock.QuantityAvailable > 0)
        {
            _context.InventoryTransactions.Add(new InventoryTransaction
            {
                Id = Guid.NewGuid(),
                StockId = stock.Id,
                QuantityChange = stock.QuantityAvailable,
                Type = InventoryTransactionType.Adjustment,
                ReferenceId = stock.Id,
                TransactionDate = DateTime.UtcNow,
                PerformedById = userId,
                Remarks = "Opening stock quantity."
            });
        }

        await _context.SaveChangesAsync();

        return ToStockDto(stock, category);
    }

    public async Task<InventoryStockDto> UpdateStockAsync(Guid stockId, UpdateInventoryStockDto dto, Guid userId)
    {
        var stock = await _context.InventoryStocks
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == stockId);

        if (stock == null)
            throw new Exception("Stock item not found.");

        var category = await _context.InventoryCategories.FirstOrDefaultAsync(x => x.Id == dto.CategoryId);

        if (category == null)
            throw new Exception("Invalid category.");

        stock.Name = dto.Name.Trim();
        stock.CategoryId = dto.CategoryId;
        stock.Unit = string.IsNullOrWhiteSpace(dto.Unit) ? stock.Unit : dto.Unit.Trim();
        stock.ReorderLevel = dto.ReorderLevel < 0 ? stock.ReorderLevel : dto.ReorderLevel;

        await _context.SaveChangesAsync();

        return ToStockDto(stock, category);
    }

    public async Task AdjustStockAsync(Guid stockId, AdjustInventoryStockDto dto, Guid userId)
    {
        var stock = await _context.InventoryStocks.FirstOrDefaultAsync(x => x.Id == stockId);

        if (stock == null)
            throw new Exception("Stock item not found.");

        if (dto.QuantityChange == 0)
            throw new Exception("Quantity change cannot be zero.");

        if (stock.QuantityAvailable + dto.QuantityChange < 0)
            throw new Exception("Stock quantity cannot become negative.");

        stock.QuantityAvailable += dto.QuantityChange;

        _context.InventoryTransactions.Add(new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            StockId = stock.Id,
            QuantityChange = dto.QuantityChange,
            Type = InventoryTransactionType.Adjustment,
            ReferenceId = stock.Id,
            TransactionDate = DateTime.UtcNow,
            PerformedById = userId,
            Remarks = dto.Remarks
        });

        await _context.SaveChangesAsync();
    }

    public async Task<List<InventoryStockDto>> GetAvailableStocksAsync()
    {
        return await _context.InventoryStocks
            .Include(x => x.Category)
            .AsNoTracking()
            .Where(x => x.QuantityAvailable > 0)
            .OrderBy(x => x.Name)
            .Select(x => new InventoryStockDto
            {
                Id = x.Id,
                Name = x.Name,
                SKU = x.SKU,
                QuantityAvailable = x.QuantityAvailable,
                CategoryId = x.CategoryId,
                CategoryName = x.Category.Name,
                Unit = x.Unit,
                ReorderLevel = x.ReorderLevel,
                IsAssetTracked = x.Category.IsAssetTracked,
                Status =
                    x.QuantityAvailable == 0 ? "OutOfStock" :
                    x.QuantityAvailable <= x.ReorderLevel ? "LowStock" :
                    "InStock"
            })
            .ToListAsync();
    }

    // ============================================================
    // ASSET ITEMS
    // ============================================================

    public async Task<InventoryPagedResponse> GetInventoryAsync(InventoryQueryParams query)
    {
        var itemsQuery = _context.InventoryItems
        .Include(x => x.InventoryCategory)
        .Include(x => x.Stock)
        .Include(x => x.AssignedTo)
        .AsNoTracking()
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(query.Search))
    {
        var search = query.Search.Trim().ToLower();

        itemsQuery = itemsQuery.Where(x =>
            x.Name.ToLower().Contains(search) ||
            x.SKU.ToLower().Contains(search) ||
            (x.Barcode != null && x.Barcode.ToLower().Contains(search)) ||
            (x.SerialNumber != null && x.SerialNumber.ToLower().Contains(search)));
    }

    if (query.CategoryId.HasValue)
    {
        itemsQuery = itemsQuery.Where(x => x.InventoryCategoryId == query.CategoryId.Value);
    }

    if (!string.IsNullOrWhiteSpace(query.Status))
    {
        itemsQuery = query.Status switch
        {
            "Assigned" => itemsQuery.Where(x => x.Status == InventoryItemStatus.Assigned),
            "Available" => itemsQuery.Where(x => x.Status == InventoryItemStatus.Available),
            "Maintenance" => itemsQuery.Where(x => x.Status == InventoryItemStatus.Maintenance),
            "Damaged" => itemsQuery.Where(x => x.Status == InventoryItemStatus.Damaged),
            "Lost" => itemsQuery.Where(x => x.Status == InventoryItemStatus.Lost),
            "Retired" => itemsQuery.Where(x => x.Status == InventoryItemStatus.Retired),
            _ => itemsQuery
        };
    }

    var total = await itemsQuery.CountAsync();

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
            InventoryCategoryId = x.InventoryCategoryId,

            SerialNumber = x.SerialNumber,
            Barcode = x.Barcode,

            Status = x.Status,
            Condition = x.Condition,

            Description = x.Description,

            Location = x.Location,

            AssignedTo = x.AssignedTo != null ? x.AssignedTo.FullName : null
        })
        .ToListAsync();

    return new InventoryPagedResponse
    {
        Items = data,
        TotalCount = total,
        PageNumber = query.PageNumber,
        PageSize = query.PageSize,
        Stats = new InventoryStatsDto
        {
            TotalItems = await _context.InventoryItems.CountAsync(),
            Assigned = await _context.InventoryItems.CountAsync(x => x.Status == InventoryItemStatus.Assigned),
            Available = await _context.InventoryItems.CountAsync(x => x.Status == InventoryItemStatus.Available),
            Maintenance = await _context.InventoryItems.CountAsync(x => x.Status == InventoryItemStatus.Maintenance)
        }
    };
    }

    public async Task<InventoryItemDto> CreateItemAsync(CreateInventoryItemDto dto, Guid userId)
    {
        var stock = await _context.InventoryStocks
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == dto.StockId);

        if (stock == null)
            throw new Exception("Stock not found.");

        if (!stock.Category.IsAssetTracked)
            throw new Exception("Cannot create physical asset for non-asset-tracked stock.");

        var sku = await GenerateSkuAsync(stock.Name, stock.CategoryId);

        var item = new InventoryItem
        {
            Id = Guid.NewGuid(),
            StockId = stock.Id,
            Name = stock.Name,
            InventoryCategoryId = stock.CategoryId,
            SKU = sku,
            Barcode = sku,
            Description = dto.Description,
            SerialNumber = dto.SerialNumber,
            CreatedById = userId,
            CreatedAt = DateTime.UtcNow,
            Status = InventoryItemStatus.Available,
            Condition = InventoryItemCondition.Good,
            Location = string.IsNullOrWhiteSpace(dto.Location) ? "Inventory" : dto.Location
        };

        stock.QuantityAvailable += 1;

        _context.InventoryItems.Add(item);

        _context.InventoryTransactions.Add(new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            StockId = stock.Id,
            QuantityChange = 1,
            Type = InventoryTransactionType.Adjustment,
            ReferenceId = item.Id,
            TransactionDate = DateTime.UtcNow,
            PerformedById = userId,
            Remarks = "Manual asset item created."
        });

        await _context.SaveChangesAsync();

        return new InventoryItemDto
        {
            Id = item.Id,
            Name = item.Name,
            SKU = item.SKU,
            SerialNumber = item.SerialNumber,
            Location = item.Location,
            Status = item.Status,
            Category = stock.Category.Name
        };
    }

    public async Task<InventoryItemDto> UpdateItemAsync(Guid itemId, UpdateInventoryItemDto dto, Guid userId)
    {
        var item = await _context.InventoryItems
            .Include(x => x.InventoryCategory)
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item == null)
            throw new Exception("Item not found.");

        item.Description = dto.Description ?? item.Description;
        item.Status = dto.Status;
        item.Condition = dto.Condition;
        item.Location = dto.Location ?? item.Location;
        item.SerialNumber = dto.SerialNumber ?? item.SerialNumber;

        await _context.SaveChangesAsync();

        return new InventoryItemDto
        {
            Id = item.Id,
            Name = item.Name,
            SKU = item.SKU,
            SerialNumber = item.SerialNumber,
            Location = item.Location,
            Status = item.Status,
            Category = item.InventoryCategory.Name
        };
    }

    public async Task<InventoryItemDetailDto> GetInventoryItemById(Guid id)
    {
        var result = await GetInventoryItemByIdAsync(id);

        if (result == null)
            throw new Exception("Item not found.");

        return result;
    }

    public async Task<InventoryItemDetailDto?> GetInventoryItemByIdAsync(Guid itemId)
    {
        var item = await _context.InventoryItems
            .Include(i => i.AssignedTo)
            .Include(i => i.InventoryCategory)
            .Include(i => i.Stock)
            .Include(i => i.AssignmentHistories)
                .ThenInclude(h => h.AssignedTo)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item == null)
            return null;

        return new InventoryItemDetailDto
        {
            Id = item.Id,
            Name = item.Name,
            SKU = item.SKU,
            Barcode = item.Barcode,
            SerialNumber = item.SerialNumber,
            Category = item.InventoryCategory.Name,
            CategoryId = item.InventoryCategory.Id,
            Status = item.Status,
            Condition = item.Condition,
            Location = item.Location,
            AssignedTo = item.AssignedTo != null ? item.AssignedTo.FullName : null,
            AssignedDate = item.AssignedDate,
            Description = item.Description,
            CreatedAt = item.CreatedAt,
            AssignmentHistory = item.AssignmentHistories
                .OrderByDescending(h => h.AssignedDate)
                .Select(h => new InventoryAssignmentHistoryDto
                {
                    UserName = h.AssignedTo != null ? h.AssignedTo.FullName : null,
                    AssignedDate = h.AssignedDate,
                    ReturnedDate = h.UnassignedDate
                })
                .ToList()
        };
    }

    public async Task<IEnumerable<InventoryItemDropDownDto>> GetItemsByCategoryAsync(Guid categoryId)
    {
        return await _context.InventoryStocks
            .Where(i => i.CategoryId == categoryId)
            .OrderBy(i => i.Name)
            .Select(i => new InventoryItemDropDownDto
            {
                Id = i.Id,
                Name = i.Name
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<InventoryItemDropDownDto>> GetAvailableAssetsByStockAsync(Guid stockId)
    {
        return await _context.InventoryItems
            .Where(i => i.StockId == stockId && i.Status == InventoryItemStatus.Available)
            .OrderBy(i => i.Name)
            .Select(i => new InventoryItemDropDownDto
            {
                Id = i.Id,
                Name = $"{i.Name} - {i.SKU}"
            })
            .ToListAsync();
    }

    public async Task<InventoryItemDetailDto?> AssignItemAsync(Guid itemId, AssignInventoryItemDto dto, Guid assignedBy)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var item = await _context.InventoryItems
            .Include(x => x.Stock)
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item == null)
            throw new Exception("Item not found.");

        if (item.Status != InventoryItemStatus.Available)
            throw new Exception("Item is not available.");

        item.AssignedToId = dto.UserId;
        item.AssignedDate = DateTime.UtcNow;
        item.Status = InventoryItemStatus.Assigned;

        if (item.Stock.QuantityAvailable > 0)
            item.Stock.QuantityAvailable -= 1;

        _context.InventoryAssignmentHistories.Add(new InventoryAssignmentHistory
        {
            Id = Guid.NewGuid(),
            InventoryItemId = item.Id,
            AssignedToId = dto.UserId,
            AssignedDate = DateTime.UtcNow,
            ActionType = "ASSIGNED",
            PerformedById = assignedBy,
            Notes = dto.Notes ?? "Item assigned to user."
        });

        _context.InventoryTransactions.Add(new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            StockId = item.StockId,
            QuantityChange = -1,
            Type = InventoryTransactionType.Issue,
            ReferenceId = item.Id,
            TransactionDate = DateTime.UtcNow,
            PerformedById = assignedBy,
            Remarks = $"Asset assigned to user {dto.UserId}."
        });

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return await GetInventoryItemByIdAsync(itemId);
    }

    public async Task<InventoryItemDetailDto?> UnassignItemAsync(Guid itemId, Guid unassignedBy)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var item = await _context.InventoryItems
            .Include(x => x.Stock)
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item == null)
            throw new Exception("Item not found.");

        if (item.Status != InventoryItemStatus.Assigned || item.AssignedToId == null)
            throw new Exception("Item is not currently assigned.");

        item.AssignedToId = null;
        item.AssignedDate = null;
        item.Status = InventoryItemStatus.Available;

        item.Stock.QuantityAvailable += 1;

        var activeHistory = await _context.InventoryAssignmentHistories
            .Where(h => h.InventoryItemId == item.Id && h.UnassignedDate == null)
            .OrderByDescending(h => h.AssignedDate)
            .FirstOrDefaultAsync();

        if (activeHistory != null)
        {
            activeHistory.UnassignedDate = DateTime.UtcNow;
            activeHistory.PerformedById = unassignedBy;
            activeHistory.ActionType = "RETURNED";
        }

        _context.InventoryTransactions.Add(new InventoryTransaction
        {
            Id = Guid.NewGuid(),
            StockId = item.StockId,
            QuantityChange = 1,
            Type = InventoryTransactionType.Return,
            ReferenceId = item.Id,
            TransactionDate = DateTime.UtcNow,
            PerformedById = unassignedBy,
            Remarks = "Asset returned to inventory."
        });

        await _context.SaveChangesAsync();
        await tx.CommitAsync();

        return await GetInventoryItemByIdAsync(itemId);
    }

    // ============================================================
    // CATEGORY
    // ============================================================

    public async Task<CategoryPagedResponse> GetCategoriesAsync(CategoryQueryParams query)
    {
        var baseQuery = _context.InventoryCategories
            .Where(c => c.ParentCategoryId == null)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();

            baseQuery = baseQuery.Where(c =>
                c.Name.ToLower().Contains(search) ||
                c.CategoryCode.ToLower().Contains(search));
        }

        var totalCount = await baseQuery.CountAsync();

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
                IsAssetTracked = c.IsAssetTracked,
                TotalItems =
                    _context.InventoryStocks.Count(s => s.CategoryId == c.Id) +
                    _context.InventoryStocks.Count(s => s.Category.ParentCategoryId == c.Id),
                SubCategories = _context.InventoryCategories
                    .Where(sc => sc.ParentCategoryId == c.Id)
                    .Select(sc => new InventoryCategoryDto
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        CategoryCode = sc.CategoryCode,
                        Description = sc.Description,
                        RiskWeight = sc.RiskWeight,
                        IsAssetTracked = sc.IsAssetTracked,
                        TotalItems = _context.InventoryStocks.Count(s => s.CategoryId == sc.Id),
                        SubCategories = new List<InventoryCategoryDto>()
                    })
                    .ToList()
            })
            .ToListAsync();

        return new CategoryPagedResponse
        {
            Categories = categories,
            CategoryStats = new CategoryStats
            {
                TotalCategories = await _context.InventoryCategories.CountAsync(c => c.ParentCategoryId == null),
                TotalSubCategories = await _context.InventoryCategories.CountAsync(c => c.ParentCategoryId != null),
                TotalItems = await _context.InventoryStocks.CountAsync()
            },
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    public async Task CreateCategoryAsync(CreateCategoryDto dto, Guid userId)
    {
        var categoryCode = dto.ParentCategoryId == null
            ? await GenerateUniqueRootCode(dto.Name)
            : await GenerateSubCategoryCode(dto.ParentCategoryId.Value);

        var category = new InventoryCategory
        {
            Id = Guid.NewGuid(),
            Name = dto.Name.Trim(),
            RiskWeight = dto.RiskWeight,
            CategoryCode = categoryCode,
            Description = dto.Description,
            ParentCategoryId = dto.ParentCategoryId,
            IsAssetTracked = dto.IsAssetTracked,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.InventoryCategories.Add(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateCategoryAsync(Guid id, UpdateInventoryCategoryDto dto, Guid userId)
    {
        var category = await _context.InventoryCategories.FirstOrDefaultAsync(x => x.Id == id);

        if (category == null)
            throw new Exception("Category not found.");

        var hasStock = await _context.InventoryStocks.AnyAsync(x => x.CategoryId == id);
        var hasAssets = await _context.InventoryItems.AnyAsync(x => x.InventoryCategoryId == id);

        if ((hasStock || hasAssets) && category.IsAssetTracked != dto.IsAssetTracked)
            throw new Exception("Cannot change asset tracking type after stock or assets exist.");

        category.Name = dto.Name;
        category.Description = dto.Description;
        category.RiskWeight = dto.RiskWeight;
        category.IsAssetTracked = dto.IsAssetTracked;
        category.UpdatedAt = DateTime.UtcNow;
        category.UpdatedBy = userId;

        if (category.ParentCategoryId == null && !string.IsNullOrWhiteSpace(dto.CategoryCode))
            category.CategoryCode = dto.CategoryCode.Trim().ToUpper();

        await _context.SaveChangesAsync();
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _context.InventoryCategories
            .Include(c => c.SubCategories)
            .Include(c => c.Stocks)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category == null)
            throw new Exception("Category not found.");

        if (category.SubCategories.Any())
            throw new Exception("Cannot delete category with subcategories.");

        if (category.Stocks.Any())
            throw new Exception("Cannot delete category with stock items.");

        _context.InventoryCategories.Remove(category);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<CategoryDto>> GetLeafCategoriesAsync()
    {
        return await _context.InventoryCategories
            .Where(c => !c.SubCategories.Any())
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name
            })
            .ToListAsync();
    }

    public async Task<List<InventoryCategoryDto>> GetLeafCategories()
    {
        return await _context.InventoryCategories
            .Where(c => !c.SubCategories.Any())
            .OrderBy(c => c.Name)
            .Select(c => new InventoryCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                CategoryCode = c.CategoryCode,
                IsAssetTracked = c.IsAssetTracked
            })
            .ToListAsync();
    }

    // ============================================================
    // SKU HELPERS
    // ============================================================

    public async Task<string> GenerateSkuAsync(string name, Guid categoryId)
    {
        var category = await _context.InventoryCategories.FirstOrDefaultAsync(c => c.Id == categoryId);

        if (category == null)
            throw new Exception("Category not found.");

        var categoryCode = category.CategoryCode;
        var nameCode = new string(name.ToUpper().Where(char.IsLetterOrDigit).ToArray());

        nameCode = nameCode.Length <= 3 ? nameCode : nameCode[..3];

        var prefix = $"{categoryCode}-{nameCode}";

        var itemCount = await _context.InventoryItems.CountAsync(i => i.SKU.StartsWith(prefix));
        var stockCount = await _context.InventoryStocks.CountAsync(i => i.SKU.StartsWith(prefix));

        return $"{prefix}-{itemCount + stockCount + 1:D3}";
    }

    private InventoryStockDto ToStockDto(InventoryStock stock, InventoryCategory category)
    {
        return new InventoryStockDto
        {
            Id = stock.Id,
            Name = stock.Name,
            SKU = stock.SKU,
            QuantityAvailable = stock.QuantityAvailable,
            CategoryId = stock.CategoryId,
            CategoryName = category.Name,
            Unit = stock.Unit,
            ReorderLevel = stock.ReorderLevel,
            IsAssetTracked = category.IsAssetTracked,
            Status =
                stock.QuantityAvailable == 0 ? "OutOfStock" :
                stock.QuantityAvailable <= stock.ReorderLevel ? "LowStock" :
                "InStock"
        };
    }

    private string GenerateCodeFromName(string name)
    {
        var words = name.Trim().ToUpper().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var code = string.Join("", words.Select(w => w[0]));

        if (code.Length < 4)
        {
            var all = string.Join("", words);
            code += all[..Math.Min(4 - code.Length, all.Length)];
        }

        return code.Length > 4 ? code[..4] : code;
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

    private async Task<string> GenerateSubCategoryCode(Guid parentCategoryId)
    {
        var parent = await _context.InventoryCategories.FirstOrDefaultAsync(x => x.Id == parentCategoryId);

        if (parent == null)
            throw new Exception("Parent category not found.");

        var lastChild = await _context.InventoryCategories
            .Where(x => x.ParentCategoryId == parentCategoryId)
            .OrderByDescending(x => x.CategoryCode)
            .FirstOrDefaultAsync();

        int nextNumber = 1;

        if (lastChild != null)
        {
            var parts = lastChild.CategoryCode.Split('-');
            if (int.TryParse(parts.Last(), out var num))
                nextNumber = num + 1;
        }

        return $"{parent.CategoryCode}-{nextNumber:D3}";
    }
    
    public Task ReceiveFromPurchaseOrderAsync(Guid purchaseOrderId)
    {
        return ReceiveFromPurchaseOrderAsync(purchaseOrderId, null);
    }

}