using System.ComponentModel.DataAnnotations;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities.Inventory;


public class ProcurementRequest
{
    public Guid Id { get; set; }

    public Guid InventoryRequestId { get; set; }
    public InventoryRequest InventoryRequest { get; set; } = null!;

    public Guid RequestedById { get; set; }
    public User RequestedBy { get; set; } = null!;

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public Guid ApprovedByManagerId { get; set; }
    public User ApprovedByManager { get; set; } = null!;

    public ProcurementRequestStatus Status { get; set; } = ProcurementRequestStatus.Pending;

    public string? Remarks { get; set; }
    public string? RejectionReason { get; set; }

    public Guid? RequisitionId { get; set; }
    public Requisition? Requisition { get; set; }

    public DateTime ApprovedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ProcurementRequestItem> Items { get; set; } = new List<ProcurementRequestItem>();
}