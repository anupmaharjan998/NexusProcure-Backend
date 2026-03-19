using NexusProcure.Core.Enums;

namespace NexusProcure.Core.DTOs.PurchaseOrder;

public class PurchaseOrderDto
{
    public Guid Id { get; set; }

    public string PoNumber { get; set; } = string.Empty;
    public string ReqNumber { get; set; } = string.Empty;

    public string VendorName { get; set; } = string.Empty;

    public string? VendorEmail { get; set; }
    public string? VendorContactPerson { get; set; }
    public string? VendorPhoneNumber { get; set; }
    public string? VendorAddress { get; set; }
    
    public string PaymentTerms { get; set; }

    public DateTime PoDate { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string DeliveryStatus { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal TotalAmount { get; set; }

    public List<PurchaseOrderItemDto> Items { get; set; } = new();
}


public class PurchaseOrderItemDto
{
    public string ItemName { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal TaxPercentage { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}