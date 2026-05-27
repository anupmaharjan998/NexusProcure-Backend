using System;
using System.Collections.Generic;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities.Inventory;

public class InventoryItem : BaseEntity
{
    public Guid Id { get; set; }
    
    public Guid StockId { get; set; }
    public InventoryStock? Stock { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    
    public string? SerialNumber { get; set; }
    
    public string SKU { get; set; }
    public string Barcode { get; set; }
    
    public InventoryItemStatus Status { get; set; } // Available / Assigned / Maintenance
    public InventoryItemCondition Condition { get; set; } // Good / Damaged
    
    public string Location { get; set; }
    
    public Guid? AssignedToId { get; set; }
    public User AssignedTo { get; set; }
    
    public DateTime? AssignedDate { get; set; }
    
    public Guid InventoryCategoryId { get; set; }
    public InventoryCategory InventoryCategory { get; set; }


    public Guid? CreatedById { get; set; }
    public User CreatedBy { get; set; }
    
    public DateTime UpdatedAt { get; set; }

    public ICollection<InventoryAssignmentHistory> AssignmentHistories { get; set; }
}