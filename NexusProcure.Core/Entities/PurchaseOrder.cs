using NexusProcure.Core.Entities.RequestForQuotations;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities;

public class PurchaseOrder : BaseEntity
{
    public Guid Id { get; set; }
    public string PurchaseOrderNumber { get; set; } = string.Empty;

    public Guid RequisitionId { get; set; }
    public Requisition Requisition { get; set; }

    public Guid VendorId { get; set; }
    public Vendor Vendor { get; set; }

    public Guid QuotationId { get; set; }
    public Quotation Quotation { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Issued;

    public DeliveryStatus DeliveryStatus { get; set; } = DeliveryStatus.Pending;

    public decimal SubTotal { get; set; }
    public decimal Vat { get; set; }
    public decimal TotalAmount { get; set; }

    // Navigation
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}
