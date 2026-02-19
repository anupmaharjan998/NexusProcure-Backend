namespace NexusProcure.Core.DTOs.RFQ;

public class QuotationSubmitDto
{
    public List<QuotationItemSubmitDto> Items { get; set; } = [];
    public string Notes { get; set; } = null!;
    public string Signature { get; set; } = null!;
    public DateTime DeliveryTime { get; set; }
    
}


public class QuotationItemSubmitDto
{
    public Guid RfqItemId { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
}
