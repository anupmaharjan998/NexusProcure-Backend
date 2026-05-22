namespace NexusProcure.Core.Enums;

public enum InventoryRequestStatus
{
    PendingManagerApproval = 1,
    ApprovedByManager = 2,
    RejectedByManager = 3,
    Completed = 4,
    PartiallyIssued = 5,
    SentForProcurement = 6,
    Cancelled = 7
}

public enum ProcurementStatus
{
    PendingApproval,
    Approved,
    Rejected
}

public enum RequestPriority
{
    Low,
    Medium,
    High,
    Critical
}