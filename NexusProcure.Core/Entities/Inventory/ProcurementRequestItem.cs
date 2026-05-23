using System;

namespace NexusProcure.Core.Entities.Inventory;

public class ProcurementRequestItem
{
    public Guid Id { get; set; }

    public Guid ProcurementRequestId { get; set; }
    public ProcurementRequest ProcurementRequest { get; set; }

    public Guid ItemId { get; set; }
    public int Quantity { get; set; }
}