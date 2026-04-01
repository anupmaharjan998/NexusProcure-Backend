namespace NexusProcure.Core.Entities.Inventory;

public class InventoryAssignment
{
    public Guid Id { get; set; }

    public Guid InventoryItemId { get; set; }
    public InventoryItem InventoryItem { get; set; }

    public Guid UserId { get; set; } // your existing User

    public int Quantity { get; set; }

    public DateTime AssignedDate { get; set; }

    public Guid ReferenceId { get; set; } // RequisitionId

    public bool IsReturned { get; set; }
}