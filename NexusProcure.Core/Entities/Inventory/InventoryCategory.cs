using System;
using System.Collections.Generic;

namespace NexusProcure.Core.Entities.Inventory;

public class InventoryCategory : BaseEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string CategoryCode { get; set; } // Unique
    public string? Description { get; set; }
    public int RiskWeight { get; set; }
    
    public bool IsAssetTracked { get; set; }

    // Self-reference
    public Guid? ParentCategoryId { get; set; }
    public InventoryCategory ParentInventoryCategory { get; set; }
    
    public Guid CreatedBy { get; set; }

    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    
    public ICollection<InventoryCategory> SubCategories { get; set; }
    public ICollection<InventoryStock> Stocks { get; set; } = new List<InventoryStock>();
    
    public ICollection<InventoryItem> Items { get; set; }
    public ICollection<Vendor>? Vendors { get; set; }
    public ICollection<VendorCategory> VendorCategories { get; set; } = new List<VendorCategory>();
}