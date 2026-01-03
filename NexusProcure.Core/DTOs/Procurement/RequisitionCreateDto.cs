using NexusProcure.Application.Services.Procurement;

namespace NexusProcure.Core.DTOs.Procurement;

public class RequisitionCreateDto
{
    public Guid RequestedById { get; set; }
    public Guid CategoryId { get; set; }   // ✅ ADD
    public bool IsUrgent { get; set; }      // ✅ ADD
    public List<RequisitionItemDto> Items { get; set; } = new List<RequisitionItemDto>();
}

public class RequisitionItemDto
{
    public Guid Id { get; set; }
    public Guid RequisitionId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal EstimatedCost { get; set; }
}

public class RequisitionResponseDto
{
    public Guid Id { get; set; }
    public Guid RequestedById { get; set; }
    public UserResponseDto RequestedBy { get; set; }
    
    public decimal TotalAmount { get; set; }
    public string RequestedByName { get; set; }

    public DateTime RequestedDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected

    // Navigation
    public List<RequisitionItemDto> Items { get; set; }
    public List<ApprovalDto> Approvals { get; set; }
    // public List<PurchaseOrderDto> PurchaseOrders { get; set; }

}
