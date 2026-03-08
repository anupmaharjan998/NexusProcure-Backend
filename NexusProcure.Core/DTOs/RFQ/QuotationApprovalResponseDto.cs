using NexusProcure.Application.Services.Procurement;

namespace NexusProcure.Core.DTOs.RFQ;

public class QuotationApprovalItemDto
{
    public Guid Id { get; set; }
    public Guid QuotationId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal EstimatedCost { get; set; }
    

    public decimal TaxPercentage { get; set; }
    public decimal UnitPrice { get; set; }

    public int DeliveryDays { get; set; }

    public string? Remarks { get; set; }

}

public class QuotationApprovalResponseDto
{
    public Guid Id { get; set; }
    public string RfqNumber { get; set; }
    public Guid RequestedById { get; set; }
    public UserResponseDto RequestedBy { get; set; }
    
    public decimal TotalAmount { get; set; }
    public string RequestedByName { get; set; }

    public DateTime RequestedDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    
    public string CategoryName { get; set; }

    // Navigation
    public List<QuotationApprovalItemDto> Items { get; set; }
    public List<ApprovalDto> Approvals { get; set; }
    // public List<PurchaseOrderDto> PurchaseOrders { get; set; }

}



public class QuotationApprovalListResponseDto
{
    public Guid Id { get; set; }
    public string RfqNumber { get; set; }
    public string VendorName { get; set; }
    public string ContactPerson { get; set; }
    
    public decimal TotalAmount { get; set; }

    public DateTime RequestedDate { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    
    //public string CategoryName { get; set; }
    // public List<PurchaseOrderDto> PurchaseOrders { get; set; }

}