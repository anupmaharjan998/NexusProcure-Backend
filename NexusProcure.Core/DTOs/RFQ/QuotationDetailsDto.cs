namespace NexusProcure.Core.DTOs.RFQ;

public class QuotationDetailsDto
{
    public Guid Id { get; set; }
    public string VendorName { get; set; } = null!;
    public string VendorEmail { get; set; } = null!;
    public string ContactPerson { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
    public DateTime DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public List<QuotationItemsDto> Items { get; set; } = new();
    public string Status { get; set; } = null!;
}

public class QuotationItemsDto
{
    public Guid Id { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal Total { get; set; }
}