using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.RFQ;

public class QuotationDto
{
    public Guid Id { get; set; }
    public string VendorName { get; set; } = null!;
    public string VendorEmail { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
    public DateTime DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsSelected { get; set; }
    public string Status { get; set; } = null!;
}

public class QuotationSummaryDto
{
    public int Total { get; set; }
    public decimal Lowest { get; set; }
    public decimal Highest { get; set; }
    public decimal Average { get; set; }
}

public class QuotationListResponseDto
{
    public QuotationSummaryDto Summary { get; set; } = null!;
    public List<QuotationDto> Quotations { get; set; } = new();
}

