using System;

namespace NexusProcure.Core.DTOs.Inventory;

public class CreateInventoryItemDto
{
    public string Name { get; set; }
    public Guid StockId { get; set; }
    public string Description { get; set; }
    public Guid CategoryId { get; set; }
    public string Location { get; set; }
    public string? SerialNumber { get; set; }
}