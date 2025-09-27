namespace NexusProcure.Core.Entities;

public class RequisitionItem
{
    public Guid Id { get; set; }
    public Guid RequisitionId { get; set; }
    public Requisition Requisition { get; set; }

    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal EstimatedCost { get; set; }
}
