namespace NexusProcure.Core.DTOs.RFQ;

public class QuotationExcelRowDto
{
    public string ItemName { get; set; } = default!;
    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }
    public decimal TaxPercentage { get; set; }
}
