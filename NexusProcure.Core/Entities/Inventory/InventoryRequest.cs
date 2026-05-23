using System;
using System.Collections.Generic;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities.Inventory;

public class InventoryRequest : BaseEntity
{
    public Guid Id { get; set; }

    public Guid RequestedById { get; set; }
    public User RequestedBy { get; set; }

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; }

    public string Purpose { get; set; } = string.Empty;
    public RequestPriority Priority { get; set; }

    public InventoryRequestStatus Status { get; set; } = InventoryRequestStatus.PendingManagerApproval;

    public Guid? ApprovedByManagerId { get; set; }
    public User? ApprovedByManager { get; set; }

    public Guid? ProcessedByInventoryManagerId { get; set; }
    public User? ProcessedByInventoryManager { get; set; }

    public string? Remarks { get; set; }

    public ICollection<InventoryRequestItem> Items { get; set; } = new List<InventoryRequestItem>();
}