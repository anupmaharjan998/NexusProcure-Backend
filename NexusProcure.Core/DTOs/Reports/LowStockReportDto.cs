namespace NexusProcure.Core.DTOs.Reports;

public class LowStockReportDto
{
    public Guid StockId { get; set; }

    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;

    public decimal AvailableQuantity { get; set; }
    public decimal ReorderLevel { get; set; }

    public string StockStatus { get; set; } = string.Empty;
}