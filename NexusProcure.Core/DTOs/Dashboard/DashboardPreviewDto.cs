namespace NexusProcure.Core.DTOs.Dashboard;

public class AssignedInventoryPreviewDto
{
    public Guid AssignmentId { get; set; }
    public Guid InventoryItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public string? SerialNumber { get; set; }
    public string? Barcode { get; set; }
    public string? Location { get; set; }
    public string? Condition { get; set; }
    public int Quantity { get; set; }
    public DateTime? AssignedAt { get; set; }
}

public class LowStockPreviewDto
{
    public Guid InventoryItemId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public string? Location { get; set; }
}

public class PendingApprovalPreviewDto
{
    public Guid ApprovalId { get; set; }
    public Guid RequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public string RequestedBy { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? RequestedAt { get; set; }
}

public class MyRequisitionPreviewDto
{
    public Guid RequisitionId { get; set; }
    public string RequisitionNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public DateTime? CreatedAt { get; set; }
}