namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryAssignmentHistoryDto
{
    public string UserName { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
}