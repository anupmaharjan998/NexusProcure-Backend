namespace NexusProcure.Core.DTOs.RFQ;

public class ParsedQuotationItemDto
{
    public string ItemName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
}
