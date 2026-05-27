namespace NexusProcure.Core.DTOs.Reports;

public class PurchaseOrderReportQuery
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public Guid? VendorId { get; set; }

    public string? Status { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}