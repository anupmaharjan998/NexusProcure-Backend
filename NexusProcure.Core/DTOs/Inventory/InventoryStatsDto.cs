namespace NexusProcure.Core.DTOs.Inventory;

public class InventoryStatsDto
{
    public int TotalItems { get; set; }
    public int Assigned { get; set; }
    public int Available { get; set; }
    public int Maintenance { get; set; }
}