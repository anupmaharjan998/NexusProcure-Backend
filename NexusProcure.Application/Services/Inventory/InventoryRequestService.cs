using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Inventory;

public class InventoryRequestService : IInventoryRequestService
{
    private readonly NexusProcureDbContext _context;
    private readonly IDelegationService _delegationService;

    public InventoryRequestService(
        NexusProcureDbContext context,
        IDelegationService delegationService)
    {
        _context = context;
        _delegationService = delegationService;
    }

    public async Task<Guid> CreateAsync(Guid userId, CreateInventoryRequestDto dto)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new Exception("User not found.");

        if (user.DepartmentId == null)
            throw new Exception("User must belong to a department.");

        if (dto.Items == null || !dto.Items.Any())
            throw new Exception("At least one item is required.");

        var requestItems = new List<InventoryRequestItem>();

        foreach (var item in dto.Items)
        {
            if (item.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            var stock = await _context.InventoryStocks
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == item.StockId);

            if (stock == null)
                throw new Exception("Stock item not found.");

            requestItems.Add(new InventoryRequestItem
            {
                Id = Guid.NewGuid(),
                StockId = stock.Id,
                QuantityRequested = item.Quantity,
                QuantityIssued = 0
            });
        }

        var request = new InventoryRequest
        {
            Id = Guid.NewGuid(),
            RequestedById = userId,
            DepartmentId = user.DepartmentId.Value,
            Purpose = dto.Purpose,
            Priority = dto.Priority,
            Status = InventoryRequestStatus.PendingManagerApproval,
            Items = requestItems,
            CreatedAt = DateTime.UtcNow
        };

        _context.InventoryRequests.Add(request);
        await _context.SaveChangesAsync();

        return request.Id;
    }

    public async Task ApproveByManagerAsync(Guid requestId, Guid approverId)
    {
        var request = await _context.InventoryRequests
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null)
            throw new Exception("Request not found.");

        if (request.Status != InventoryRequestStatus.PendingManagerApproval)
            throw new Exception("Request is not pending manager approval.");

        var managerId = request.RequestedBy.ManagerId
            ?? throw new Exception("Manager not assigned.");

        var delegateUser = await _delegationService.GetActiveDelegateAsync(managerId);
        var validApprover = delegateUser?.Id ?? managerId;

        if (approverId != validApprover)
            throw new Exception("Unauthorized.");

        request.Status = InventoryRequestStatus.ApprovedByManager;
        request.ApprovedByManagerId = approverId;

        await _context.SaveChangesAsync();
    }

    public async Task RejectByManagerAsync(Guid requestId, Guid approverId, string? remarks = null)
    {
        var request = await _context.InventoryRequests
            .Include(r => r.RequestedBy)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null)
            throw new Exception("Request not found.");

        if (request.Status != InventoryRequestStatus.PendingManagerApproval)
            throw new Exception("Request is not pending manager approval.");

        var managerId = request.RequestedBy.ManagerId
            ?? throw new Exception("Manager not assigned.");

        var delegateUser = await _delegationService.GetActiveDelegateAsync(managerId);
        var validApprover = delegateUser?.Id ?? managerId;

        if (approverId != validApprover)
            throw new Exception("Unauthorized.");

        request.Status = InventoryRequestStatus.RejectedByManager;
        request.ApprovedByManagerId = approverId;
        request.Remarks = remarks;

        await _context.SaveChangesAsync();
    }

    public async Task ProcessByInventoryManagerAsync(
        Guid requestId,
        Guid inventoryManagerId,
        ProcessInventoryRequestDto dto)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var request = await _context.InventoryRequests
            .Include(r => r.Items)
                .ThenInclude(i => i.Stock)
                    .ThenInclude(s => s.Category)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request == null)
            throw new Exception("Request not found.");

        if (request.Status != InventoryRequestStatus.ApprovedByManager)
            throw new Exception("Request is not approved by manager.");

        var allAvailable = true;

        foreach (var requestItem in request.Items)
        {
            var stock = requestItem.Stock;
            var category = stock.Category;

            if (category.IsAssetTracked)
            {
                var dtoItem = dto.Items
                    .FirstOrDefault(x => x.InventoryRequestItemId == requestItem.Id);

                if (dtoItem == null || dtoItem.InventoryItemIds.Count == 0)
                    throw new Exception($"Asset selection required for {stock.Name}.");

                if (dtoItem.InventoryItemIds.Count > requestItem.QuantityRequested)
                    throw new Exception($"Selected assets exceed requested quantity for {stock.Name}.");

                var selectedAssets = await _context.InventoryItems
                    .Where(x =>
                        dtoItem.InventoryItemIds.Contains(x.Id) &&
                        x.StockId == stock.Id &&
                        x.Status == InventoryItemStatus.Available)
                    .ToListAsync();

                if (selectedAssets.Count != dtoItem.InventoryItemIds.Count)
                    throw new Exception($"Some selected assets for {stock.Name} are not available.");

                requestItem.QuantityIssued = selectedAssets.Count;

                if (requestItem.QuantityIssued < requestItem.QuantityRequested)
                    allAvailable = false;

                foreach (var asset in selectedAssets)
                {
                    asset.Status = InventoryItemStatus.Assigned;
                    asset.AssignedToId = request.RequestedById;
                    asset.AssignedDate = DateTime.UtcNow;

                    _context.InventoryAssignmentHistories.Add(new InventoryAssignmentHistory
                    {
                        Id = Guid.NewGuid(),
                        InventoryItemId = asset.Id,
                        AssignedToId = request.RequestedById,
                        AssignedDate = DateTime.UtcNow,
                        ActionType = "ASSIGNED",
                        PerformedById = inventoryManagerId,
                        Notes = $"Issued through inventory request {request.Id}"
                    });

                    _context.InventoryRequestIssuedItems.Add(new InventoryRequestIssuedItem
                    {
                        Id = Guid.NewGuid(),
                        InventoryRequestItemId = requestItem.Id,
                        InventoryItemId = asset.Id
                    });
                }

                stock.QuantityAvailable -= requestItem.QuantityIssued;

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    StockId = stock.Id,
                    QuantityChange = -requestItem.QuantityIssued,
                    Type = InventoryTransactionType.Issue,
                    ReferenceId = request.Id,
                    TransactionDate = DateTime.UtcNow,
                    PerformedById = inventoryManagerId,
                    Remarks = "Asset issued to user."
                });
            }
            else
            {
                if (stock.QuantityAvailable >= requestItem.QuantityRequested)
                {
                    requestItem.QuantityIssued = requestItem.QuantityRequested;
                    stock.QuantityAvailable -= requestItem.QuantityRequested;
                }
                else
                {
                    requestItem.QuantityIssued = stock.QuantityAvailable;
                    stock.QuantityAvailable = 0;
                    allAvailable = false;
                }

                _context.InventoryTransactions.Add(new InventoryTransaction
                {
                    Id = Guid.NewGuid(),
                    StockId = stock.Id,
                    QuantityChange = -requestItem.QuantityIssued,
                    Type = InventoryTransactionType.Issue,
                    ReferenceId = request.Id,
                    TransactionDate = DateTime.UtcNow,
                    PerformedById = inventoryManagerId,
                    Remarks = "Consumable stock issued to user."
                });
            }
        }

        request.ProcessedByInventoryManagerId = inventoryManagerId;
        request.Status = allAvailable
            ? InventoryRequestStatus.Completed
            : InventoryRequestStatus.SentForProcurement;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async Task<InventoryRequestDto?> GetByIdAsync(Guid requestId)
    {
        return await _context.InventoryRequests
            .Include(x => x.RequestedBy)
            .Include(x => x.Department)
            .Include(x => x.Items)
                .ThenInclude(x => x.Stock)
                    .ThenInclude(x => x.Category)
            .AsNoTracking()
            .Where(x => x.Id == requestId)
            .Select(x => new InventoryRequestDto
            {
                Id = x.Id,
                RequestedBy = x.RequestedBy.FullName,
                Department = x.Department.DepartmentName,
                Purpose = x.Purpose,
                Priority = x.Priority.ToString(),
                Status = x.Status.ToString(),
                Items = x.Items.Select(i => new InventoryRequestItemDto
                {
                    Id = i.Id,
                    StockId = i.StockId,
                    StockName = i.Stock.Name,
                    QuantityRequested = i.QuantityRequested,
                    QuantityIssued = i.QuantityIssued,
                    IsAssetTracked = i.Stock.Category.IsAssetTracked
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
}