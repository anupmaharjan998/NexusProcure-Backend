namespace NexusProcure.Core.DTOs.ProcurementRequest;

public class ProcurementRequestItemDto
{
    public Guid Id { get; set; }
    public Guid StockId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int RequestedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int RequiredProcurementQuantity { get; set; }
    public string? Notes { get; set; }
}