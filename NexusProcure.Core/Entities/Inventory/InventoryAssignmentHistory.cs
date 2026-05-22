using System;

namespace NexusProcure.Core.Entities.Inventory;

public class InventoryAssignmentHistory
{
    public Guid Id { get; set; }

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;

    public Guid AssignedToId { get; set; }
    public User AssignedTo { get; set; } = null!;

    public DateTime AssignedDate { get; set; }
    public DateTime? UnassignedDate { get; set; }

    public string ActionType { get; set; }

    public Guid PerformedById { get; set; }
    public User PerformedBy { get; set; } = null!;

    public string? Notes { get; set; }
}   