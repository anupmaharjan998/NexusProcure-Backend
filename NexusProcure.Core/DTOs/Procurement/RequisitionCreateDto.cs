namespace NexusProcure.Core.DTOs.Procurement;

public class RequisitionCreateDto
{
    public Guid RequestedById { get; set; }

    public bool IsUrgent { get; set; }

    public string Purpose { get; set; } = string.Empty;

    public DateTime? RequiredDate { get; set; }

    public string? Notes { get; set; }

    public List<RequisitionItemCreateDto> Items { get; set; } = new();
}

public class RequisitionItemCreateDto
{
    public Guid InventoryStockId { get; set; }

    public int Quantity { get; set; }

    public decimal EstimatedCost { get; set; }

    public string? Remarks { get; set; }
}