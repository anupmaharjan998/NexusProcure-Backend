using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Email;
using NexusProcure.Application.Interfaces;
using NexusProcure.Application.Interfaces.Procurement;
using NexusProcure.Application.Interfaces.ProcurementRequest;
using NexusProcure.Core.DTOs.Email;
using NexusProcure.Core.DTOs.Procurement;
using NexusProcure.Core.DTOs.ProcurementRequest;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Enums;
using NexusProcure.Infrastructure.Data;

namespace NexusProcure.Application.Services.ProcurementRequest;

public class ProcurementRequestService : IProcurementRequestService
{
    private readonly NexusProcureDbContext _context;
    private readonly IEmailService _emailService;
    private readonly IRequisitionService _requisitionService;

    public ProcurementRequestService(
        NexusProcureDbContext context,
        IEmailService emailService,
        IRequisitionService requisitionService)
    {
        _context = context;
        _emailService = emailService;
        _requisitionService = requisitionService;
    }

    public async Task<Core.Entities.Inventory.ProcurementRequest> CreateFromApprovedInventoryRequestAsync(
        Guid inventoryRequestId,
        Guid approvedByManagerId,
        string? managerRemarks)
    {
        var request = await _context.InventoryRequests
            .Include(x => x.RequestedBy)
            .Include(x => x.Department)
            .Include(x => x.Items)
                .ThenInclude(x => x.Stock)
                    .ThenInclude(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == inventoryRequestId);

        if (request == null)
            throw new InvalidOperationException("Inventory request not found.");

        var manager = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == approvedByManagerId);

        if (manager == null)
            throw new InvalidOperationException("Manager not found.");

        var alreadyExists = await _context.ProcurementRequests
            .AnyAsync(x => x.InventoryRequestId == inventoryRequestId);

        if (alreadyExists)
            throw new InvalidOperationException("Procurement request already exists for this requisition.");

        var shortageItems = request.Items
            .Select(item =>
            {
                var availableQty = item.Stock.QuantityAvailable;
                var requestedQty = item.QuantityRequested;
                var requiredProcurementQty = Math.Max(requestedQty - availableQty, 0);

                return new
                {
                    Item = item,
                    AvailableQuantity = availableQty,
                    RequestedQuantity = requestedQty,
                    RequiredProcurementQuantity = requiredProcurementQty
                };
            })
            .Where(x => x.RequiredProcurementQuantity > 0)
            .ToList();

        if (!shortageItems.Any())
            throw new InvalidOperationException("No shortage items found. Procurement is not required.");

        var procurementRequest = new Core.Entities.Inventory.ProcurementRequest
        {
            Id = Guid.NewGuid(),
            InventoryRequestId = request.Id,
            RequestedById = request.RequestedById,
            ApprovedByManagerId = approvedByManagerId,
            ApprovedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            Status = ProcurementRequestStatus.Pending,
            Remarks = managerRemarks,
            DepartmentId = request.DepartmentId,
            Items = shortageItems.Select(x => new ProcurementRequestItem
            {
                Id = Guid.NewGuid(),
                InventoryRequestItemId = x.Item.Id,
                InventoryStockId = x.Item.StockId,
                RequestedQuantity = x.RequestedQuantity,
                AvailableQuantity = x.AvailableQuantity,
                RequiredProcurementQuantity = x.RequiredProcurementQuantity,
            }).ToList()
        };

        _context.ProcurementRequests.Add(procurementRequest);

        request.Status = InventoryRequestStatus.SentForProcurement;

        await _context.SaveChangesAsync();

        await SendProcurementEmailAsync(procurementRequest.Id);

        return procurementRequest;
    }

    private async Task SendProcurementEmailAsync(Guid procurementRequestId)
    {
        var procurementRequest = await _context.ProcurementRequests
            .Include(x => x.InventoryRequest)
            .ThenInclude(x => x.Department)
            .Include(x => x.RequestedBy)
            .Include(x => x.ApprovedByManager)
            .Include(x => x.Items)
            .ThenInclude(x => x.InventoryStock)
            .FirstOrDefaultAsync(x => x.Id == procurementRequestId);

        if (procurementRequest == null)
            return;

        var procurementOfficers = await _context.Users
            .Include(x => x.Role)
            .Where(x =>
                x.Role.Name == "ProcurementOfficer" ||
                x.Role.Name == "Procurement Officer")
            .ToListAsync();

        if (!procurementOfficers.Any())
            return;

        var emailItems = procurementRequest.Items.Select(x => (
            itemName: x.InventoryStock.Name,
            requestedQty: x.RequestedQuantity,
            availableQty: x.AvailableQuantity,
            procureQty: x.RequiredProcurementQuantity
        ));

        foreach (var officer in procurementOfficers)
        {
            if (string.IsNullOrWhiteSpace(officer.Email))
                continue;

            var subject = ProcurementEmailTemplates.ProcurementRequestSubject(
                procurementRequest.InventoryRequest.Department.DepartmentName
            );

            var body = ProcurementEmailTemplates.ProcurementRequestBody(
                officer.FullName,
                procurementRequest.InventoryRequest.Department.DepartmentName,
                procurementRequest.RequestedBy.FullName,
                procurementRequest.ApprovedByManager.FullName,
                emailItems
            );

            await _emailService.SendAsync(new SendEmailDto
            {
                To = officer.Email,
                Subject = subject,
                HtmlBody = body,
            });
        }
    }

    public async Task<List<ProcurementRequestListDto>> GetAllAsync()
    {
        return await _context.ProcurementRequests
            .AsNoTracking()
            .Include(x => x.InventoryRequest)
            .Include(x => x.RequestedBy)
            .Include(x => x.ApprovedByManager)
            .Include(x => x.Items)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new ProcurementRequestListDto
            {
                Id = x.Id,
                InventoryRequestId = x.InventoryRequestId,
                RequestedBy = x.RequestedBy.FullName,
                ApprovedByManager = x.ApprovedByManager.FullName,
                ApprovedAt = x.ApprovedAt,
                CreatedAt = x.CreatedAt,
                Status = x.Status.ToString(),
                TotalItems = x.Items.Count,
                TotalQuantityToProcure = x.Items.Sum(i => i.RequiredProcurementQuantity)
            })
            .ToListAsync();
    }

    public async Task<ProcurementRequestDetailsDto?> GetByIdAsync(Guid id)
    {
        return await _context.ProcurementRequests
            .AsNoTracking()
            .Include(x => x.InventoryRequest)
                .ThenInclude(x => x.Department)
            .Include(x => x.RequestedBy)
            .Include(x => x.ApprovedByManager)
            .Include(x => x.Items)
                .ThenInclude(x => x.InventoryStock)
                    .ThenInclude(x => x.Category)
            .Where(x => x.Id == id)
            .Select(x => new ProcurementRequestDetailsDto
            {
                Id = x.Id,
                InventoryRequestId = x.InventoryRequestId,
                RequestedBy = x.RequestedBy.FullName,
                Department = x.InventoryRequest.Department.DepartmentName,
                ApprovedByManager = x.ApprovedByManager.FullName,
                ApprovedAt = x.ApprovedAt,
                CreatedAt = x.CreatedAt,
                Status = x.Status.ToString(),
                ManagerRemarks = x.Remarks,
                Items = x.Items.Select(i => new ProcurementRequestItemDto
                {
                    Id = i.Id,
                    StockId = i.InventoryStockId,
                    ItemName = i.InventoryStock.Name,
                    CategoryName = i.InventoryStock.Category.Name,
                    RequestedQuantity = i.RequestedQuantity,
                    AvailableQuantity = i.AvailableQuantity,
                    RequiredProcurementQuantity = i.RequiredProcurementQuantity,
                    Notes = i.Notes
                }).ToList()
            })
            .FirstOrDefaultAsync();
    }
    
    public async Task<Guid> CreateRequisitionAsync(
    Guid procurementRequestId,
    Guid procurementOfficerId,
    CreateRequisitionFromProcurementRequestDto dto)
    {
        if (dto == null)
            throw new InvalidOperationException("Create requisition payload is required.");

        if (dto.Items == null || !dto.Items.Any())
            throw new InvalidOperationException("At least one item with estimated cost is required.");

        var procurementRequest = await _context.ProcurementRequests
            .Include(x => x.Department)
            .Include(x => x.InventoryRequest)
            .Include(x => x.Items)
                .ThenInclude(x => x.InventoryStock)
            .FirstOrDefaultAsync(x => x.Id == procurementRequestId);

        if (procurementRequest == null)
            throw new InvalidOperationException("Procurement request not found.");

        if (procurementRequest.Status != ProcurementRequestStatus.Pending)
            throw new InvalidOperationException("Only pending procurement requests can be converted to requisition.");

        if (procurementRequest.RequisitionId != null)
            return procurementRequest.RequisitionId.Value;

        if (procurementRequest.DepartmentId == Guid.Empty)
            throw new InvalidOperationException("Procurement request department is missing.");

        if (procurementRequest.Items == null || !procurementRequest.Items.Any())
            throw new InvalidOperationException("No items found in procurement request.");

        var costByItemId = dto.Items.ToDictionary(
            x => x.ProcurementRequestItemId,
            x => x
        );

        var validItems = procurementRequest.Items
            .Where(x => x.RequiredProcurementQuantity > 0)
            .ToList();

        if (!validItems.Any())
            throw new InvalidOperationException("No valid procurement quantity found.");

        foreach (var item in validItems)
        {
            if (!costByItemId.TryGetValue(item.Id, out var costItem))
                throw new InvalidOperationException($"Estimated cost is missing for item {item.InventoryStock.Name}.");

            if (costItem.EstimatedUnitCost <= 0)
                throw new InvalidOperationException($"Estimated unit cost must be greater than zero for {item.InventoryStock.Name}.");
        }

        var departmentName = procurementRequest.Department?.DepartmentName ?? "Unknown Department";

        var inventoryPurpose = string.IsNullOrWhiteSpace(procurementRequest.InventoryRequest?.Purpose)
            ? "Inventory shortage procurement"
            : procurementRequest.InventoryRequest.Purpose.Trim();

        var priority = procurementRequest.InventoryRequest?.Priority.ToString();

        var requisitionDto = new RequisitionCreateDto
        {
            RequestedById = procurementOfficerId,
            RequiredDate = ToUtcDate(dto.RequiredDate),
            IsUrgent =
                string.Equals(priority, "High", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(priority, "Urgent", StringComparison.OrdinalIgnoreCase),
            Purpose = $"Procurement for inventory request from department {departmentName}: {inventoryPurpose}",
            Notes = string.IsNullOrWhiteSpace(dto.Notes)
                ? procurementRequest.Remarks
                : dto.Notes.Trim(),
            DepartmentId = procurementRequest.DepartmentId,
            Items = validItems.Select(item =>
            {
                var costItem = costByItemId[item.Id];

                return new RequisitionItemCreateDto
                {
                    InventoryStockId = item.InventoryStockId,
                    Quantity = item.RequiredProcurementQuantity,
                    EstimatedCost = costItem.EstimatedUnitCost,
                    Remarks = string.IsNullOrWhiteSpace(costItem.Remarks)
                        ? item.Notes
                        : costItem.Remarks.Trim()
                };
            }).ToList()
        };

        var createdRequisition = await _requisitionService.CreateAsync(requisitionDto);

        if (createdRequisition == null)
            throw new InvalidOperationException("Requisition creation failed.");

        procurementRequest.Status = ProcurementRequestStatus.RequisitionCreated;
        procurementRequest.RequisitionId = createdRequisition.Id;
        procurementRequest.Remarks = "Requisition created from procurement request.";
        procurementRequest.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return createdRequisition.Id;
    }
    
    
    
    public async Task RejectAsync(
        Guid procurementRequestId,
        Guid procurementOfficerId,
        string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("Reject reason is required.");

        var procurementRequest = await _context.ProcurementRequests
            .FirstOrDefaultAsync(x => x.Id == procurementRequestId);

        if (procurementRequest == null)
            throw new InvalidOperationException("Procurement request not found.");

        if (procurementRequest.Status != ProcurementRequestStatus.Pending)
            throw new InvalidOperationException("Only pending procurement requests can be rejected.");

        procurementRequest.Status = ProcurementRequestStatus.Rejected;
        procurementRequest.Remarks = reason.Trim();
        procurementRequest.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }
    
    private static DateTime ToUtcDate(DateTime? value)
    {
        if (!value.HasValue)
            return DateTime.UtcNow.AddDays(7);
    
        return value.Value.Kind switch
        {
            DateTimeKind.Utc => value.Value,
            DateTimeKind.Local => value.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
        };
    }
}