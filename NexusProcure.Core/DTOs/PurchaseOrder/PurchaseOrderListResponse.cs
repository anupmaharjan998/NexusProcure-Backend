namespace NexusProcure.Core.DTOs.PurchaseOrder;

public class PurchaseOrderListResponse
{
    public int TotalPOs { get; set; }

    public decimal TotalValue { get; set; }

    public int InTransit { get; set; }

    public int Delivered { get; set; }

    public List<PurchaseOrderDto> Orders { get; set; } = new();
}