using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities;

public class ApprovalPolicy
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }
    public InventoryCategory Category { get; set; }
    
    public RiskLevel RiskLevel { get; set; }

    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public int SequenceOrder { get; set; }
    public int EscalationHours { get; set; } // SLA

    public bool IsActive { get; set; }
}

