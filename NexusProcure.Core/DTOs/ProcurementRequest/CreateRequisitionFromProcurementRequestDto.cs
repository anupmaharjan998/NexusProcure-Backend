namespace NexusProcure.Core.DTOs.ProcurementRequest;

public class CreateRequisitionFromProcurementRequestDto
{
    public DateTime? RequiredDate { get; set; }
    public string? Notes { get; set; }

    public List<CreateRequisitionFromProcurementRequestItemDto> Items { get; set; } = new();
}

public class CreateRequisitionFromProcurementRequestItemDto
{
    public Guid ProcurementRequestItemId { get; set; }
    public decimal EstimatedUnitCost { get; set; }
    public string? Remarks { get; set; }
}