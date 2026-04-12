using NexusProcure.Core.Entities.Inventory;

namespace NexusProcure.Core.Entities;

public class VendorCategory
{
    public Guid VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    public Guid CategoryId { get; set; }
    public InventoryCategory Category { get; set; } = null!;
}