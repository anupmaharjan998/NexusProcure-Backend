using System;

namespace NexusProcure.Core.DTOs.Delegation;

public class DelegationDto
{
    public Guid Id { get; set; }

    public Guid DelegatorUserId { get; set; }
    public string DelegatorName { get; set; } = string.Empty;
    public string? DelegatorEmail { get; set; }

    public Guid DelegateUserId { get; set; }
    public string DelegateName { get; set; } = string.Empty;
    public string? DelegateEmail { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Scope { get; set; } = "All";

    public string? Reason { get; set; }

    public bool IsActive { get; set; }

    public bool IsExpired { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}