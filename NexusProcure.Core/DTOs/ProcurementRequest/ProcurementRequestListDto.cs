namespace NexusProcure.Core.DTOs.ProcurementRequest;

public class ProcurementRequestListDto
{
    public Guid Id { get; set; }
    public Guid InventoryRequestId { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public string ApprovedByManager { get; set; } = string.Empty;
    public DateTime ApprovedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public int TotalQuantityToProcure { get; set; }
}