namespace NexusProcure.Core.Entities;

public class AssetAssignment
{
    public Guid Id { get; set; }
    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; }

    public Guid AssignedToId { get; set; }
    public User AssignedTo { get; set; }

    public DateTime AssignedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public string ConditionOnReturn { get; set; } = string.Empty;
}
