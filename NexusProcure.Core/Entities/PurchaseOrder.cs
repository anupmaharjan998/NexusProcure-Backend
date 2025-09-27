namespace NexusProcure.Core.Entities;

public class PurchaseOrder
{
    public Guid Id { get; set; }

    public Guid RequisitionId { get; set; }
    public Requisition Requisition { get; set; }

    public Guid VendorId { get; set; }
    public Vendor Vendor { get; set; }

    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = "Open"; // Open, Completed, Cancelled

    // Navigation
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
