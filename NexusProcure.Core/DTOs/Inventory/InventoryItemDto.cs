using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryItemDto
{
    public Guid Id { get; set; }

    public string SKU { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;
    public Guid InventoryCategoryId { get; set; }

    public string? SerialNumber { get; set; }
    public string? Barcode { get; set; }

    public InventoryItemStatus Status { get; set; }
    public InventoryItemCondition Condition { get; set; }

    public string? Description { get; set; }

    public string? AssignedTo { get; set; }

    public string Location { get; set; } = string.Empty;
}

public class InventoryItemDropDownDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}