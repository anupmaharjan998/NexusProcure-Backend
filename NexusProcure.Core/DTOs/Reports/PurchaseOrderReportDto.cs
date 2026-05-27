namespace NexusProcure.Core.DTOs.Reports;

public class PurchaseOrderReportDto
{
    public Guid Id { get; set; }

    public string PoNumber { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public string RequisitionNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal PendingQuantity { get; set; }

    public bool IsTodayDelivery { get; set; }
    public bool IsOverdue { get; set; }
}