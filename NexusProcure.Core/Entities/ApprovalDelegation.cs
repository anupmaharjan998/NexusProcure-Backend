namespace NexusProcure.Application.Interfaces;

public class ApprovalDelegation
{
    public Guid Id { get; set; }

    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}
