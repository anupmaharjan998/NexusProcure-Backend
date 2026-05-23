using System;
using System.Collections.Generic;
using NexusProcure.Core.Enums;

namespace NexusProcure.Core.Entities.Inventory;

public class GoodsReceipt
{
    public Guid Id { get; set; }

    public Guid PurchaseOrderId { get; set; }
    public PurchaseOrder PurchaseOrder { get; set; } = null!;

    public DateTime ReceivedDate { get; set; }

    public Guid ReceivedById { get; set; }
    public User ReceivedBy { get; set; } = null!;

    public string? Notes { get; set; }

    public InventoryProcessingStatus InventoryProcessingStatus { get; set; } = InventoryProcessingStatus.Pending;
    public string? InventoryProcessingError { get; set; }

    public ICollection<GoodsReceiptItem> Items { get; set; } = new List<GoodsReceiptItem>();
}