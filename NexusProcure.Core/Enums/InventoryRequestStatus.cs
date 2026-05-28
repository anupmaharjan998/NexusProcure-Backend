namespace NexusProcure.Core.Enums;

public enum InventoryRequestStatus
{
    Draft = 0,
    Submitted = 1,
    PendingManagerApproval = 2,
    PendingManagerProcurementDecision = 3,
    ManagerApproved = 4,
    ManagerRejected = 5,
    SentForProcurement = 6,
    ProcurementInProgress = 7,
    Completed = 8,
    Cancelled = 9,
    RejectedInsufficientQuantity = 10
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