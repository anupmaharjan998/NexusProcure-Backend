using NexusProcure.Application.Services.Procurement;

namespace NexusProcure.Core.DTOs.Procurement;

public class RequisitionResponseDto
{
    public Guid Id { get; set; }

    public string RequisitionNumber { get; set; } = string.Empty;

    public Guid RequestedById { get; set; }

    public UserResponseDto RequestedBy { get; set; } = null!;

    public string RequestedByName { get; set; } = string.Empty;

    public DateTime RequestedDate { get; set; }

    public DateTime? RequiredDate { get; set; }

    public string Status { get; set; } = "Pending";

    public bool IsUrgent { get; set; }

    public string Purpose { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public decimal TotalAmount { get; set; }

    public int RiskScore { get; set; }

    public string RiskLevel { get; set; } = string.Empty;

    public string CategoryName { get; set; } = string.Empty;

    public List<RequisitionItemDto> Items { get; set; } = new();

    public List<ApprovalDto> Approvals { get; set; } = new();
}