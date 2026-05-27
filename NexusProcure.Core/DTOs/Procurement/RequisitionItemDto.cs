namespace NexusProcure.Core.DTOs.Procurement;

public class RequisitionItemDto
{
    public Guid Id { get; set; }

    public Guid RequisitionId { get; set; }

    public Guid InventoryStockId { get; set; }

    public string ItemName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;

    public decimal EstimatedCost { get; set; }

    public decimal LineTotal { get; set; }

    public string? Remarks { get; set; }
}