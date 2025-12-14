namespace NexusProcure.Application.Services.Procurement;

public class ApprovalDto
{
    public Guid Id { get; set; }
    public Guid RequisitionId { get; set; }

    public Guid ApprovedById { get; set; }

    public DateTime ApprovedDate { get; set; }
    public string Decision { get; set; } 
    public string Comments { get; set; }
}