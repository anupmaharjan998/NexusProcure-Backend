namespace NexusProcure.Core.Entities;

public class Requisition
{
    public Guid Id { get; set; }
    public Guid RequestedById { get; set; }
    public User RequestedBy { get; set; }

    public DateTime RequestedDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } // Low, Medium, High
    
    public bool IsUrgent { get; set; } 
    

    // Navigation
    public ICollection<RequisitionItem> Items { get; set; } = new List<RequisitionItem>();
    public ICollection<Approval> Approvals { get; set; } = new List<Approval>();

    // 👇 This is what was missing
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
