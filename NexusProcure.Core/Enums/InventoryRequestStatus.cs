namespace NexusProcure.Core.Enums;

public enum InventoryRequestStatus
{
    PendingManagerApproval = 1,
    ApprovedByManager = 2,
    RejectedByManager = 3,
    Completed = 4,
    PendingManagerProcurementDecision = 5,
    SentForProcurement = 6,
    RejectedInsufficientQuantity = 7
}

public enum ProcurementStatus
{
    PendingApproval,
    Approved,
    Rejected
}

public enum RequestPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Urgent = 4
}