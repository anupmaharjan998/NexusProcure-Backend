namespace NexusProcure.Core.Entities.Inventory;

public class InventoryItem : BaseEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public string? SerialNumber { get; set; }
    public string SKU { get; set; }
    public string Barcode { get; set; }
    
    public string Status { get; set; } // Available / Assigned / Maintenance
    
    public Guid? AssignedToId { get; set; }
    public User AssignedTo { get; set; }
    public DateTime AssignedDate { get; set; }
    public string Location { get; set; }
    public string Condition { get; set; } // Good / Damaged
    
    public Guid InventoryCategoryId { get; set; }
    public InventoryCategory InventoryCategory { get; set; }


    public Guid? CreatedById { get; set; }
    public User CreatedBy { get; set; }

    public ICollection<InventoryAssignmentHistory> AssignmentHistories { get; set; }
}