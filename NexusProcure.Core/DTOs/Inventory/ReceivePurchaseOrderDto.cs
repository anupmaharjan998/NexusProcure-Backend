public class ReceivePurchaseOrderDto
{
    public Guid PurchaseOrderId { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public DateTime? NextExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }

    public List<ReceivePurchaseOrderItemDto> Items { get; set; } = new();
}

public class ReceivePurchaseOrderItemDto
{
    public Guid PurchaseOrderItemId { get; set; }
    public int QuantityReceived { get; set; }

    public string? Location { get; set; }
    public string? Condition { get; set; }
    public string? Notes { get; set; }

    public List<ReceiveAssetDetailDto> AssetDetails { get; set; } = new();
}

public class ReceiveAssetDetailDto
{
    public string? SerialNumber { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Condition { get; set; }
}