using Microsoft.EntityFrameworkCore;
using NexusProcure.Application.Interfaces;
using NexusProcure.Core.DTOs;
using NexusProcure.Core.Entities;
using NexusProcure.Core.Entities.Inventory;
using NexusProcure.Core.Entities.RequestForQuotations;
using NexusProcure.Infrastructure.Data.Seeds;
using InventoryItem = NexusProcure.Core.Entities.Inventory.InventoryItem;

namespace NexusProcure.Infrastructure.Data;

public class NexusProcureDbContext : DbContext
{
    public NexusProcureDbContext(DbContextOptions options) : base(options)
    {
        
    }

    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Department> Departments { get; set; }

    public DbSet<Requisition> Requisitions { get; set; }
    public DbSet<RequisitionItem> RequisitionItems { get; set; }
    public DbSet<Approval> Approvals { get; set; }
    public DbSet<ApprovalLevel> ApprovalLevels { get; set; }
    public DbSet<ApprovalPolicy> ApprovalPolicies { get; set; }


    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<VendorDocument> VendorDocuments { get; set; }
    
    public DbSet<AssetAssignment> AssetAssignments { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    
    //public DbSet<Category> Categories { get; set; }
    public DbSet<ApprovalDelegation> ApprovalDelegations { get; set; }
    
    public DbSet<TotalAmountRiskScore> TotalAmountRiskScores { get; set; }
    
    public DbSet<Quotation> Quotations { get; set; }
    public DbSet<QuotationItem> QuotationItems { get; set; }
    public DbSet<RequestForQuotation> RequestForQuotations { get; set; }
    public DbSet<RfqVendor> RfqVendors { get; set; }
    public DbSet<RfqAudit> RfqAudits { get; set; }
    public DbSet<RfqAccessToken> RfqAccessTokens { get; set; }
    public DbSet<InventoryCategory> InventoryCategories { get; set; }
    public DbSet<InventoryItem> InventoryItems { get; set; }
    // public DbSet<Stock> Stocks { get; set; }
    // public DbSet<StockTransaction> StockTransactions { get; set; }
    public DbSet<GoodsReceipt> GoodsReceipts { get; set; }
    public DbSet<GoodsReceiptItem> GoodsReceiptItems { get; set; }
    public DbSet<InventoryAssignment> InventoryAssignments { get; set; }
    public DbSet<InventoryAssignmentHistory> InventoryAssignmentHistories { get; set; }
    public DbSet<UserDelegation> UserDelegations { get; set; }
    public DbSet<InventoryRequest> InventoryRequests { get; set; }
    public DbSet<InventoryRequestItem> InventoryRequestItems { get; set; }
    public DbSet<InventoryStock> InventoryStocks { get; set; }
    public DbSet<ProcurementRequest> ProcurementRequests { get; set; }
    public DbSet<ProcurementRequestItem> ProcurementRequestItems { get; set; }
    public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
    
    public DbSet<InventoryRequestIssuedItem> InventoryRequestIssuedItems { get; set; }
    
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        //Dashboard 
        //Stored Procedure
        modelBuilder.Entity<DashboardStatsDto>().HasNoKey();
        modelBuilder.Entity<RecentPurchaseOrderDto>().HasNoKey();
        modelBuilder.Entity<DeliveryDto>().HasNoKey();
        
        /* ===============================
           Requisition Number Sequence
        =============================== */
        modelBuilder.HasSequence<long>("requisition_number_seq")
            .StartsAt(1)
            .IncrementsBy(1);

        /* ===============================
           Requisition Configuration
        =============================== */
        modelBuilder.Entity<Requisition>(entity =>
        {
            entity.Property(r => r.RequisitionNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.HasIndex(r => r.RequisitionNumber)
                .IsUnique();
        });

        // Requisition → Items (1-M)
        modelBuilder.Entity<Requisition>()
            .HasMany(r => r.Items)
            .WithOne(i => i.Requisition)
            .HasForeignKey(i => i.RequisitionId)
            .OnDelete(DeleteBehavior.Cascade);
        

        
        // PR → PO 
        modelBuilder.HasSequence<long>("purchase_order_number_seq")
            .StartsAt(1)
            .IncrementsBy(1);
        
        
        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(po => po.Requisition)
            .WithMany(r => r.PurchaseOrders)
            .HasForeignKey(po => po.RequisitionId);
        
        modelBuilder.Entity<Requisition>()
            .Property(r => r.RiskLevel)
            .HasConversion<string>()     // 👈 stores enum NAME
            .HasMaxLength(20)
            .IsRequired();

        
        // User → Department (1-M)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        
        // User → Role (1-M)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // User
        modelBuilder.Entity<User>()
            .HasOne(u => u.Manager)
            .WithMany(u => u.Subordinates)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // User Delegation
        modelBuilder.Entity<UserDelegation>()
            .HasOne(d => d.User)
            .WithMany(u => u.Delegations)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserDelegation>()
            .HasOne(d => d.DelegateUser)
            .WithMany()
            .HasForeignKey(d => d.DelegateUserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Vendor → Category (optional 1-M)

        modelBuilder.Entity<VendorCategory>()
            .HasKey(vc => new { vc.VendorId, vc.CategoryId });

        modelBuilder.Entity<VendorCategory>()
            .HasOne(vc => vc.Vendor)
            .WithMany(v => v.VendorCategories)
            .HasForeignKey(vc => vc.VendorId);

        modelBuilder.Entity<VendorCategory>()
            .HasOne(vc => vc.Category)
            .WithMany(c => c.VendorCategories)
            .HasForeignKey(vc => vc.CategoryId);
        
        
        // Enum conversions for Vendor
        modelBuilder.Entity<Vendor>()
            .Property(v => v.PaymentTerms)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Vendor>()
            .Property(v => v.TaxType)
            .HasConversion<string>()
            .HasMaxLength(10);
        
        // Vendor → Documents (1-M)
        modelBuilder.Entity<VendorDocument>()
            .HasOne(d => d.Vendor)
            .WithMany(v => v.Documents)
            .HasForeignKey(d => d.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        
        // Department → Head (Self-Reference)
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Head)
            .WithMany()
            .HasForeignKey(d => d.HeadId)
            .OnDelete(DeleteBehavior.Restrict);

        
        // InventoryItem → AssetAssignments (1-M)
        modelBuilder.Entity<AssetAssignment>()
            .HasOne(a => a.InventoryItem)
            .WithMany(i => i.AssetAssignments)
            .HasForeignKey(a => a.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        
        // RolePermission (Composite Key)
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);
        
        //RFQ
        modelBuilder.Entity<RfqVendor>()
            .HasIndex(v => v.AccessToken)
            .IsUnique();
        
        modelBuilder.Entity<Quotation>()
            .HasOne(q => q.RequestForQuotation)
            .WithMany(r => r.Quotations)
            .HasForeignKey(q => q.RfqId)
            .OnDelete(DeleteBehavior.Restrict);


        /* ===============================
           RFQ Number Sequence
        =============================== */
        modelBuilder.HasSequence<long>("rfq_number_seq")
            .StartsAt(1)
            .IncrementsBy(1);

        
        //Inventory
        
        

        // Unique Category Code
        modelBuilder.Entity<InventoryCategory>()
            .HasIndex(x => x.CategoryCode)
            .IsUnique();

        // Self-reference
        modelBuilder.Entity<InventoryCategory>()
            .HasOne(c => c.ParentInventoryCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<InventoryStock>()
            .HasIndex(x => new { x.Name, x.CategoryId })
            .IsUnique();
        
        modelBuilder.Entity<InventoryStock>()
            .HasMany(x => x.Items)
            .WithOne(x => x.Stock)
            .HasForeignKey(x => x.StockId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<InventoryItem>()
            .HasIndex(x => x.SKU)
            .IsUnique();
        
        modelBuilder.Entity<GoodsReceipt>()
            .HasOne(gr => gr.PurchaseOrder)
            .WithMany()
            .HasForeignKey(gr => gr.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GoodsReceipt>()
            .HasOne(gr => gr.ReceivedBy)
            .WithMany()
            .HasForeignKey(gr => gr.ReceivedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GoodsReceiptItem>()
            .HasOne(gri => gri.GoodsReceipt)
            .WithMany(gr => gr.Items)
            .HasForeignKey(gri => gri.GoodsReceiptId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GoodsReceiptItem>()
            .HasOne(gri => gri.PurchaseOrderItem)
            .WithMany()
            .HasForeignKey(gri => gri.PurchaseOrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GoodsReceiptItem>()
            .HasOne(gri => gri.InventoryItem)
            .WithMany()
            .HasForeignKey(gri => gri.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<GoodsReceipt>()
            .Property(gr => gr.InventoryProcessingStatus)
            .HasConversion<string>()
            .HasMaxLength(20);
        
        
        modelBuilder.Entity<InventoryAssignmentHistory>()
            .HasOne(h => h.InventoryItem)
            .WithMany(i => i.AssignmentHistories)
            .HasForeignKey(h => h.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InventoryAssignmentHistory>()
            .HasOne(h => h.AssignedTo)
            .WithMany()
            .HasForeignKey(h => h.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<InventoryAssignmentHistory>()
            .HasOne(h => h.PerformedBy)
            .WithMany()
            .HasForeignKey(h => h.PerformedById)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<InventoryItem>()
            .HasOne(i => i.AssignedTo)
            .WithMany()
            .HasForeignKey(i => i.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<InventoryRequestItem>()
            .HasOne(x => x.InventoryRequest)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.InventoryRequestId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<InventoryRequestIssuedItem>()
            .HasOne(x => x.InventoryRequestItem)
            .WithMany(x => x.IssuedItems)
            .HasForeignKey(x => x.InventoryRequestItemId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<InventoryRequestIssuedItem>()
            .HasOne(x => x.InventoryItem)
            .WithMany()
            .HasForeignKey(x => x.InventoryItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<InventoryTransaction>()
            .HasOne(x => x.Stock)
            .WithMany(x => x.Transactions)
            .HasForeignKey(x => x.StockId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // SEEDS
        SeedRoles(modelBuilder);
        SeedAdminUser(modelBuilder);
        PermissionSeed.Seed(modelBuilder);
    }

    private void SeedRoles(ModelBuilder modelBuilder)
    {
        var adminRoleId = Guid.Parse("c76abcb8-63b5-4e14-8428-3a9a9b7ad001");
        var ceoRoleId = Guid.Parse("d27f6b43-9f64-4b13-a289-fd7744f2f102");
        var procurementRoleId = Guid.Parse("b38b2e23-6a7e-4c6d-9d5e-437a78c7b203");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "Admin" },
            new Role { Id = ceoRoleId, Name = "CEO" },
            new Role { Id = procurementRoleId, Name = "ProcurementOfficer" }
        );
    }

    private void SeedAdminUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("a87f3d2b-0f0d-4b4e-9d2a-4e09d68f4104"),
                Username = "admin",
                Email = "admin@nexusprocure.com",
                PasswordHash = "AQAAAAIAAYagAAAAEHsHTY55ymmyC5FW7c6RpK2s/HWufLsNpUswO1iSjCFPadhi/WF+HZo86Twk4Rl4NQ==",
                RoleId = Guid.Parse("c76abcb8-63b5-4e14-8428-3a9a9b7ad001")
            },
            new User
            {
                Id = Guid.Parse("a87f3d2b-0f0d-4b4e-9d2a-4e09d68f4103"),
                Username = "admin",
                Email = "admin@mail.com",
                PasswordHash = "AQAAAAIAAYagAAAAEHsHTY55ymmyC5FW7c6RpK2s/HWufLsNpUswO1iSjCFPadhi/WF+HZo86Twk4Rl4NQ==",
                RoleId = Guid.Parse("c76abcb8-63b5-4e14-8428-3a9a9b7ad001")
            }
        );
    }
}
