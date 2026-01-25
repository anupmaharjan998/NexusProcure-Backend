namespace NexusProcure.Core.DTOs.RFQ;

public class QuotationSubmitDto
{
    public List<QuotationItemSubmitDto> Items { get; set; } = [];
}


public class QuotationItemSubmitDto
{
    public Guid RfqItemId { get; set; }
    public string ItemName { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
}
