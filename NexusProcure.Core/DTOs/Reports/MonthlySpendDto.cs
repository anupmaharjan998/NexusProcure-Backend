namespace NexusProcure.Core.DTOs.Reports;

public class MonthlySpendDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
}