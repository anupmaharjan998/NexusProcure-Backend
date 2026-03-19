namespace NexusProcure.Core.DTOs.Procurement;

public class PurchaseOrderCreateDto
{
    public Guid RequisitionId { get; set; }
    public Guid VendorId { get; set; }
    public List<PurchaseOrderItemDto> Items { get; set; } = new List<PurchaseOrderItemDto>();
}

public class PurchaseOrderItemDto
{
    public string ItemName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
    
}
