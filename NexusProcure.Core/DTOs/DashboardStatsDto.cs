using System.ComponentModel.DataAnnotations.Schema;

namespace NexusProcure.Core.DTOs;

public class DashboardStatsDto
{
    [Column("total_users")]
    public int TotalUsers { get; set; }

    [Column("total_departments")]
    public int TotalDepartments { get; set; }

    [Column("total_roles")]
    public int TotalRoles { get; set; }

    // [Column("total_permissions")]
    // public int TotalPermissions { get; set; }

    [Column("total_vendors")]
    public int TotalVendors { get; set; }

    // [Column("total_inventory_items")]
    // public int TotalInventoryItems { get; set; }
    //
    // [Column("total_stock_quantity")]
    // public int TotalStockQuantity { get; set; }
    //
    // [Column("low_stock_items")]
    // public int LowStockItems { get; set; }
    //
    // [Column("assigned_items")]
    // public int AssignedItems { get; set; }

    [Column("total_requisitions")]
    public int TotalRequisitions { get; set; }

    // [Column("pending_requisitions")]
    // public int PendingRequisitions { get; set; } 
    
    [Column("pending_requisition_approvals")]
    public int PendingRequisitionApprovals { get; set; }

    // [Column("approved_requisitions")]
    // public int ApprovedRequisitions { get; set; }
    
    [Column("total_quotations")]
    public int TotalQuotations { get; set; }
    
    [Column("total_purchase_orders")]
    public int TotalPurchaseOrders { get; set; }

    [Column("active_orders")]
    public int ActivePurchaseOrders { get; set; }
    
    // [Column("pending_purchase_orders")]
    // public int PendingPurchaseOrders { get; set; }
    //
    // [Column("completed_purchase_orders")]
    // public int CompletedPurchaseOrders { get; set; }

    // [Column("active_procurements")]
    // public int ActiveProcurements { get; set; }
}
