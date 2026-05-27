using System.ComponentModel.DataAnnotations.Schema;

namespace NexusProcure.Core.DTOs;

// public class DashboardResponseDto
// {
//     public DashboardStatsDto Stats { get; set; }
//     public List<RecentPurchaseOrderDto> RecentPOs { get; set; }
//     public List<DeliveryDto> Deliveries { get; set; }
// }

public class RecentPurchaseOrderDto
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("po_number")]
    public string PoNumber { get; set; }
    
    [Column("vendor_name")]
    public string VendorName { get; set; }
    
    [Column("status")]
    public int Status { get; set; }
    
    [Column("total_amount")]
    public decimal TotalAmount { get; set; }
    
    [Column("total_items")]
    public int TotalItems { get; set; }
    
}

public class DeliveryDto
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("po_number")]
    public string PoNumber { get; set; }
    
    [Column("vendor_name")]
    public string VendorName { get; set; }
    
    [Column("delivery_date")]
    public DateTime DeliveryDate { get; set; }
    
    [Column("total_items")]
    public int TotalItems { get; set; }

    public DateTime? ExpectedDeliveryDate { get; set; }
}