namespace NexusProcure.Core.Entities.Inventory;

public class InventoryAssignmentHistory
{
    public Guid Id { get; set; }

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; } = null!;

    public Guid AssignedToId { get; set; }
    public User AssignedTo { get; set; } = null!;

    public Guid AssignedById { get; set; }
    public User AssignedBy { get; set; } = null!;

    public DateTime AssignedDate { get; set; }

    public Guid? UnassignedById { get; set; }
    public User? UnassignedBy { get; set; }

    public DateTime? UnassignedDate { get; set; }

    public string? Notes { get; set; }
}