using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities;

public class Requisition
{
    public Guid Id { get; set; }

    public string RequisitionNumber { get; set; } = string.Empty;

    public Guid RequestedById { get; set; }
    public User RequestedBy { get; set; } = null!;

    public DateTime RequestedDate { get; set; }

    public DateTime? RequiredDate { get; set; }

    public string Status { get; set; } = "Pending";

    public bool IsUrgent { get; set; }

    public string Purpose { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public int RiskScore { get; set; }

    public RiskLevel RiskLevel { get; set; }

    public decimal TotalAmount { get; set; }

    public ICollection<RequisitionItem> Items { get; set; } = new List<RequisitionItem>();

    public ICollection<Approval> Approvals { get; set; } = new List<Approval>();

    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}