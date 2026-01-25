namespace NexusProcure.Core.DTOs.RFQ;

public class QuotationResponseDto
{
    public Guid QuotationId { get; set; }
    public Guid VendorId { get; set; }
    public string VendorName { get; set; } = default!;

    public decimal TotalAmount { get; set; }
    public DateTime SubmittedAt { get; set; }

    public List<QuotationItemResponseDto> Items { get; set; } = [];
}

public class QuotationItemResponseDto
{
    public string ItemName { get; set; } = default!;
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal LineTotal { get; set; }
}
