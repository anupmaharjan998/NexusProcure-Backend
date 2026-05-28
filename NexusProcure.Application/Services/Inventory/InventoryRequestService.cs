using Hangfire;
using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.BackgroundJobs;
using NexusProcure.Application.Interfaces.Inventory;
using NexusProcure.Application.Interfaces.ProcurementRequest;
using NexusProcure.Core.DTOs.Inventory;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.Inventory;

public class InventoryRequestService : IInventoryRequestService
{
    private readonly NexusProcureDbContext _context;
    private readonly IDelegationService _delegationService;
    private readonly IProcurementRequestService _procurementRequestService;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public InventoryRequestService(
        NexusProcureDbContext context,
        IDelegationService delegationService,
        IProcurementRequestService procurementRequestService,
        IBackgroundJobClient backgroundJobClient)
    {
        _context = context;
        _delegationService = delegationService;
        _procurementRequestService = procurementRequestService;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Guid> CreateAsync(Guid userId, CreateInventoryRequestDto dto)
    {
        if (dto == null)
            throw new Exception("Request payload is required.");

        if (string.IsNullOrWhiteSpace(dto.Purpose))
            throw new Exception("Purpose is required.");

        if (dto.Items == null || !dto.Items.Any())
            throw new Exception("At least one item is required.");

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            throw new Exception("User not found.");

        if (user.DepartmentId == null)
            throw new Exception("User must belong to a department.");

        var stockIds = dto.Items
            .Select(x => x.StockId)
            .Distinct()
            .ToList();

        var stocks = await _context.InventoryStocks
            .Include(x => x.Category)
            .Where(x => stockIds.Contains(x.Id))
            .ToListAsync();

        if (stocks.Count != stockIds.Count)
            throw new Exception("One or more stock items were not found.");

        var requestItems = new List<InventoryRequestItem>();

        foreach (var item in dto.Items)
        {
            if (item.StockId == Guid.Empty)
                throw new Exception("Stock item is required.");

            if (item.Quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");

            var stock = stocks.First(x => x.Id == item.StockId);

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
            Purpose = dto.Purpose.Trim(),
            Priority = dto.Priority,
            Status = InventoryRequestStatus.PendingManagerApproval,
            Items = requestItems,
            CreatedAt = DateTime.UtcNow
        };

        _context.InventoryRequests.Add(request);
        await _context.SaveChangesAsync();

        return request.Id;
    }

    public async Task<List<InventoryRequestSummaryDto>> GetMyRequestsAsync(Guid userId)
    {
        return await _context.InventoryRequests
            .AsNoTracking()
            .Include(x => x.RequestedBy)
            .Include(x => x.Department)
            .Include(x => x.Items)
            .Where(x => x.RequestedById == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new InventoryRequestSummaryDto
            {
                Id = x.Id,
                RequestedBy = x.RequestedBy.FullName,
                Department = x.Department.DepartmentName,
                Purpose = x.Purpose,
                Priority = x.Priority.ToString(),
                Status = x.Status.ToString(),
                TotalItems = x.Items.Count,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<InventoryRequestSummaryDto>> GetPendingForManagerAsync(Guid managerId)
    {
        var delegateUserIds = await _context.UserDelegations
            .Where(x =>
                x.DelegateUserId == managerId &&
                x.IsActive &&
                x.StartDate <= DateTime.UtcNow &&
                x.EndDate >= DateTime.UtcNow)
            .Select(x => x.UserId)
            .ToListAsync();

        delegateUserIds.Add(managerId);

        return await _context.InventoryRequests
            .AsNoTracking()
            .Include(x => x.RequestedBy)
            .Include(x => x.Department)
            .Include(x => x.Items)
            .Where(x =>
                x.Status == InventoryRequestStatus.PendingManagerApproval &&
                x.RequestedBy.ManagerId != null &&
                delegateUserIds.Contains(x.RequestedBy.ManagerId.Value))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new InventoryRequestSummaryDto
            {
                Id = x.Id,
                RequestedBy = x.RequestedBy.FullName,
                Department = x.Department.DepartmentName,
                Purpose = x.Purpose,
                Priority = x.Priority.ToString(),
                Status = x.Status.ToString(),
                TotalItems = x.Items.Count,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<List<InventoryRequestSummaryDto>> GetApprovedForInventoryManagerAsync()
    {
        return await _context.InventoryRequests
            .AsNoTracking()
            .Include(x => x.RequestedBy)
            .Include(x => x.Department)
            .Include(x => x.Items)
            .Where(x => x.Status == InventoryRequestStatus.ManagerApproved)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new InventoryRequestSummaryDto
            {
                Id = x.Id,
                RequestedBy = x.RequestedBy.FullName,
                Department = x.Department.DepartmentName,
                Purpose = x.Purpose,
                Priority = x.Priority.ToString(),
                Status = x.Status.ToString(),
                TotalItems = x.Items.Count,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<InventoryRequestDto?> GetByIdAsync(Guid requestId)
    {
        return await _context.InventoryRequests
            .AsNoTracking()
            .Include(x => x.RequestedBy)
            .Include(x => x.Department)
            .Include(x => x.Items)
                .ThenInclude(x => x.Stock)
                    .ThenInclude(x => x.Category)
            .Include(x => x.Items)
                .ThenInclude(x => x.IssuedItems)
                    .ThenInclude(x => x.InventoryItem)
            .Where(x => x.Id == requestId)
            .Select(x => new InventoryRequestDto
            {
                Id = x.Id,
                RequestedById = x.RequestedById,
                RequestedBy = x.RequestedBy.FullName,
                Department = x.Department.DepartmentName,
                Purpose = x.Purpose,
                Priority = x.Priority.ToString(),
                Status = x.Status.ToString(),
                Remarks = x.Remarks,
                CreatedAt = x.CreatedAt,
                Items = x.Items.Select(i => new InventoryRequestItemDto
                {
                    Id = i.Id,
                    StockId = i.StockId,
                    StockName = i.Stock.Name,
                    CategoryName = i.Stock.Category.Name,
                    QuantityRequested = i.QuantityRequested,
                    QuantityIssued = i.QuantityIssued,
                    QuantityAvailable = i.Stock.QuantityAvailable,
                    IsAssetTracked = i.Stock.Category.IsAssetTracked,
                    IssuedItems = i.IssuedItems.Select(ii => new IssuedInventoryItemDto
                    {
                        InventoryItemId = ii.InventoryItemId,
                        SKU = ii.InventoryItem.SKU,
                        SerialNumber = ii.InventoryItem.SerialNumber
                    }).ToList()
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<AvailableInventoryItemDto>> GetAvailableAssetsByStockAsync(Guid stockId)
    {
        return await _context.InventoryItems
            .AsNoTracking()
            .Where(x =>
                x.StockId == stockId &&
                x.Status == InventoryItemStatus.Available)
            .OrderBy(x => x.SKU)
            .Select(x => new AvailableInventoryItemDto
            {
                Id = x.Id,
                SKU = x.SKU,
                SerialNumber = x.SerialNumber,
                Status = x.Status.ToString()
            })
            .ToListAsync();
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

        request.Status = InventoryRequestStatus.ManagerApproved;
        request.ApprovedByManagerId = approverId;
        request.UpdatedAt = DateTime.UtcNow;

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

        request.Status = InventoryRequestStatus.ManagerRejected;
        request.ApprovedByManagerId = approverId;
        request.Remarks = remarks;
        request.UpdatedAt = DateTime.UtcNow;

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

        if (request.Status != InventoryRequestStatus.ManagerApproved)
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

                var selectedAssetIds = dtoItem?.InventoryItemIds?.Distinct().ToList() ?? new List<Guid>();

                if (selectedAssetIds.Count > requestItem.QuantityRequested)
                    throw new Exception($"Selected assets exceed requested quantity for {stock.Name}.");

                if (!selectedAssetIds.Any())
                {
                    requestItem.QuantityIssued = 0;
                    allAvailable = false;
                    continue;
                }

                var selectedAssets = await _context.InventoryItems
                    .Where(x =>
                        selectedAssetIds.Contains(x.Id) &&
                        x.StockId == stock.Id &&
                        x.Status == InventoryItemStatus.Available)
                    .ToListAsync();

                if (selectedAssets.Count != selectedAssetIds.Count)
                    throw new Exception($"Some selected assets for {stock.Name} are not available.");

                requestItem.QuantityIssued = selectedAssets.Count;

                if (requestItem.QuantityIssued < requestItem.QuantityRequested)
                    allAvailable = false;

                foreach (var asset in selectedAssets)
                {
                    asset.Status = InventoryItemStatus.Assigned;
                    asset.AssignedToId = request.RequestedById;
                    asset.AssignedDate = DateTime.UtcNow;
                    asset.UpdatedAt = DateTime.UtcNow;

                    _context.InventoryAssignmentHistories.Add(new InventoryAssignmentHistory
                    {
                        Id = Guid.NewGuid(),
                        InventoryItemId = asset.Id,
                        AssignedToId = request.RequestedById,
                        AssignedDate = DateTime.UtcNow,
                        ActionType = "ASSIGNED",
                        PerformedById = inventoryManagerId,
                        Notes = $"Issued through inventory request {request.Id}",
                        CreatedAt = DateTime.UtcNow
                    });

                    _context.InventoryRequestIssuedItems.Add(new InventoryRequestIssuedItem
                    {
                        Id = Guid.NewGuid(),
                        InventoryRequestItemId = requestItem.Id,
                        InventoryItemId = asset.Id,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                stock.QuantityAvailable -= requestItem.QuantityIssued;
                stock.UpdatedAt = DateTime.UtcNow;

                if (requestItem.QuantityIssued > 0)
                {
                    _context.InventoryTransactions.Add(new InventoryTransaction
                    {
                        Id = Guid.NewGuid(),
                        StockId = stock.Id,
                        QuantityChange = -requestItem.QuantityIssued,
                        Type = InventoryTransactionType.Issue,
                        ReferenceId = request.Id,
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = inventoryManagerId,
                        Remarks = "Asset issued to user.",
                        CreatedAt = DateTime.UtcNow
                    });
                }
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

                stock.UpdatedAt = DateTime.UtcNow;

                if (requestItem.QuantityIssued > 0)
                {
                    _context.InventoryTransactions.Add(new InventoryTransaction
                    {
                        Id = Guid.NewGuid(),
                        StockId = stock.Id,
                        QuantityChange = -requestItem.QuantityIssued,
                        Type = InventoryTransactionType.Issue,
                        ReferenceId = request.Id,
                        TransactionDate = DateTime.UtcNow,
                        PerformedById = inventoryManagerId,
                        Remarks = "Consumable stock issued to user.",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        request.ProcessedByInventoryManagerId = inventoryManagerId;

        if (allAvailable)
        {
            request.Status = InventoryRequestStatus.Completed;
            request.Remarks = "Inventory request completed successfully.";
        }
        else
        {
            request.Status = InventoryRequestStatus.PendingManagerProcurementDecision;
            request.Remarks = "Insufficient inventory quantity. Waiting for manager decision.";
        }

        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        await tx.CommitAsync();
    }
    
    public async Task<List<InventoryRequestSummaryDto>> GetShortagePendingForManagerAsync(Guid managerId)
    {
        var delegateUserIds = await _context.UserDelegations
            .Where(x =>
                x.DelegateUserId == managerId &&
                x.IsActive &&
                x.StartDate <= DateTime.UtcNow &&
                x.EndDate >= DateTime.UtcNow)
            .Select(x => x.UserId)
            .ToListAsync();

        delegateUserIds.Add(managerId);

        return await _context.InventoryRequests
            .AsNoTracking()
            .Include(x => x.RequestedBy)
            .Include(x => x.Department)
            .Include(x => x.Items)
            .Where(x =>
                x.Status == InventoryRequestStatus.PendingManagerProcurementDecision &&
                x.RequestedBy.ManagerId != null &&
                delegateUserIds.Contains(x.RequestedBy.ManagerId.Value))
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new InventoryRequestSummaryDto
            {
                Id = x.Id,
                RequestedBy = x.RequestedBy.FullName,
                Department = x.Department.DepartmentName,
                Purpose = x.Purpose,
                Priority = x.Priority.ToString(),
                Status = x.Status.ToString(),
                TotalItems = x.Items.Count,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();
    }
    
    public async Task SendShortageToProcurementAsync(
        Guid requestId,
        Guid managerId,
        string? remarks = null)
    {
        await using var tx = await _context.Database.BeginTransactionAsync();

        var request = await _context.InventoryRequests
            .Include(x => x.RequestedBy)
            .Include(x => x.Department)
            .Include(x => x.Items)
            .ThenInclude(x => x.Stock)
            .ThenInclude(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == requestId);

        if (request == null)
            throw new Exception("Inventory request not found.");

        if (request.Status != InventoryRequestStatus.PendingManagerProcurementDecision)
            throw new Exception("Request is not waiting for manager procurement decision.");

        await ValidateManagerOrDelegateAsync(request, managerId);

        var shortageItems = request.Items
            .Where(x => x.QuantityIssued < x.QuantityRequested)
            .ToList();

        if (!shortageItems.Any())
            throw new Exception("No shortage items found for procurement.");

        request.Status = InventoryRequestStatus.SentForProcurement;
        request.Remarks = string.IsNullOrWhiteSpace(remarks)
            ? "Manager approved shortage request for procurement."
            : remarks.Trim();

        request.ApprovedByManagerId = managerId;
        request.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // await _procurementRequestService.CreateFromApprovedInventoryRequestAsync(
        //     request.Id,
        //     managerId,
        //     request.Remarks
        // );

        await tx.CommitAsync();
        _backgroundJobClient.Enqueue<IProcurementRequestJob>(
            job => job.CreateInventoryRequestAsync(request.Id, managerId, request.Remarks) );
    }
    
    public async Task RejectShortageAsync(
        Guid requestId,
        Guid managerId,
        string? remarks = null)
    {
        var request = await _context.InventoryRequests
            .Include(x => x.RequestedBy)
            .FirstOrDefaultAsync(x => x.Id == requestId);
    
        if (request == null)
            throw new Exception("Inventory request not found.");
    
        if (request.Status != InventoryRequestStatus.PendingManagerProcurementDecision)
            throw new Exception("Request is not waiting for manager procurement decision.");
    
        await ValidateManagerOrDelegateAsync(request, managerId);
    
        request.Status = InventoryRequestStatus.RejectedInsufficientQuantity;
        request.Remarks = string.IsNullOrWhiteSpace(remarks)
            ? "Rejected due to insufficient inventory quantity."
            : remarks.Trim();
    
        request.UpdatedAt = DateTime.UtcNow;
    
        await _context.SaveChangesAsync();
    }
    
    private async Task ValidateManagerOrDelegateAsync(InventoryRequest request, Guid managerId)
    {
        var actualManagerId = request.RequestedBy.ManagerId
            ?? throw new Exception("Manager not assigned.");
    
        var delegateUser = await _delegationService.GetActiveDelegateAsync(actualManagerId);
        var validApprover = delegateUser?.Id ?? actualManagerId;
    
        if (managerId != validApprover)
            throw new Exception("Unauthorized.");
    }
}