namespace NexusProcure.Core.DTOs.RFQ;

public class QuotationComparisonResponseDto
{
    public ComparisonSummaryDto Summary { get; set; }
    public List<QuotationDetailResponseDto> Quotations { get; set; }
}

public class ComparisonSummaryDto
{
    public decimal Lowest { get; set; }
    public decimal Highest { get; set; }
    public decimal Average { get; set; }
    public decimal PriceRange { get; set; }
}

public class QuotationDetailResponseDto
{
    public Guid Id { get; set; }

    public string VendorName { get; set; }
    public string VendorEmail { get; set; }
    public string ContactPerson { get; set; }

    public DateTime SubmittedAt { get; set; }
    //public DateTime ValidUntil { get; set; }

    //public decimal SubTotal { get; set; }
    
    public decimal TotalAmount { get; set; }

    //public string Status { get; set; }

    public string PaymentTerms { get; set; }
    public DateTime DeliveryTime { get; set; }
    public string Notes { get; set; }

    public List<QuotationItemDto> Items { get; set; }
}

public class QuotationItemDto
{
    public Guid Id { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal LineTotal { get; set; }
    public decimal VatAmount { get; set; }
    public decimal Total { get; set; }
}
