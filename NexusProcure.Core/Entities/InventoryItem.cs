namespace NexusProcure.Core.Entities;

public class InventoryItem
{
    public Guid Id { get; set; }
    public string AssetTag { get; set; } = string.Empty; // Barcode
    public string ItemName { get; set; } = string.Empty;
    public string Status { get; set; } = "Available"; // Available, CheckedOut, Damaged
    public int Quantity { get; set; }

    // Navigation
    public ICollection<AssetAssignment> AssetAssignments { get; set; } = new List<AssetAssignment>();
}
