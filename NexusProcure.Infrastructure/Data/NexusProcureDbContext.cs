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

        // Dashboard Stored Procedure DTOs
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
            entity.HasKey(r => r.Id);

            entity.Property(r => r.RequisitionNumber)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(r => r.RequisitionNumber)
                .IsUnique();

            entity.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(r => r.Purpose)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(r => r.Notes)
                .HasMaxLength(1000);

            entity.Property(r => r.TotalAmount)
                .HasColumnType("numeric(18,2)");

            entity.Property(r => r.RiskLevel)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            entity.HasOne(r => r.RequestedBy)
                .WithMany()
                .HasForeignKey(r => r.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(r => r.Items)
                .WithOne(i => i.Requisition)
                .HasForeignKey(i => i.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(r => r.Approvals)
                .WithOne()
                .HasForeignKey(a => a.ReferenceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(r => r.PurchaseOrders)
                .WithOne(po => po.Requisition)
                .HasForeignKey(po => po.RequisitionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* ===============================
           Requisition Item Configuration
        =============================== */
        modelBuilder.Entity<RequisitionItem>(entity =>
        {
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Quantity)
                .IsRequired();

            entity.Property(i => i.EstimatedCost)
                .HasColumnType("numeric(18,2)");

            entity.Property(i => i.Remarks)
                .HasMaxLength(500);

            entity.HasOne(i => i.Requisition)
                .WithMany(r => r.Items)
                .HasForeignKey(i => i.RequisitionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(i => i.InventoryStock)
                .WithMany()
                .HasForeignKey(i => i.InventoryStockId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(i => i.RequisitionId);
            entity.HasIndex(i => i.InventoryStockId);
        });

        /* ===============================
           Purchase Order Number Sequence
        =============================== */
        modelBuilder.HasSequence<long>("purchase_order_number_seq")
            .StartsAt(1)
            .IncrementsBy(1);

        /* ===============================
           User Configuration
        =============================== */
        modelBuilder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>()
            .HasOne(u => u.Manager)
            .WithMany(u => u.Subordinates)
            .HasForeignKey(u => u.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        /* ===============================
           User Delegation Configuration
        =============================== */
        modelBuilder.Entity<UserDelegation>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Scope)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(d => d.Reason)
                .HasMaxLength(500);

            entity.Property(d => d.IsActive)
                .HasDefaultValue(true);

            entity.Property(d => d.CreatedAt)
                .HasDefaultValueSql("NOW()");

            entity.HasOne(d => d.User)
                .WithMany(u => u.Delegations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.DelegateUser)
                .WithMany()
                .HasForeignKey(d => d.DelegateUserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(d => d.UserId);
            entity.HasIndex(d => d.DelegateUserId);
            entity.HasIndex(d => new { d.UserId, d.IsActive });
        });

        /* ===============================
           Vendor Category Configuration
        =============================== */
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

        /* ===============================
           Vendor Configuration
        =============================== */
        modelBuilder.Entity<Vendor>()
            .Property(v => v.PaymentTerms)
            .HasConversion<string>()
            .HasMaxLength(50);

        modelBuilder.Entity<Vendor>()
            .Property(v => v.TaxType)
            .HasConversion<string>()
            .HasMaxLength(10);

        modelBuilder.Entity<VendorDocument>()
            .HasOne(d => d.Vendor)
            .WithMany(v => v.Documents)
            .HasForeignKey(d => d.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        /* ===============================
           Department Configuration
        =============================== */
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Head)
            .WithMany()
            .HasForeignKey(d => d.HeadId)
            .OnDelete(DeleteBehavior.Restrict);

        /* ===============================
           Asset Assignment Configuration
        =============================== */
        modelBuilder.Entity<AssetAssignment>()
            .HasOne(a => a.InventoryItem)
            .WithMany(i => i.AssetAssignments)
            .HasForeignKey(a => a.InventoryItemId)
            .OnDelete(DeleteBehavior.Cascade);

        /* ===============================
           Role Permission Configuration
        =============================== */
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

        /* ===============================
           RFQ Configuration
        =============================== */
        modelBuilder.Entity<RfqVendor>()
            .HasIndex(v => v.AccessToken)
            .IsUnique();

        modelBuilder.Entity<Quotation>()
            .HasOne(q => q.RequestForQuotation)
            .WithMany(r => r.Quotations)
            .HasForeignKey(q => q.RfqId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.HasSequence<long>("rfq_number_seq")
            .StartsAt(1)
            .IncrementsBy(1);

        /* ===============================
           Inventory Category Configuration
        =============================== */
        modelBuilder.Entity<InventoryCategory>()
            .HasIndex(x => x.CategoryCode)
            .IsUnique();

        modelBuilder.Entity<InventoryCategory>()
            .HasOne(c => c.ParentInventoryCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        /* ===============================
           Inventory Stock Configuration
        =============================== */
        modelBuilder.Entity<InventoryStock>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => new { x.Name, x.CategoryId })
                .IsUnique();

            entity.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Items)
                .WithOne(x => x.Stock)
                .HasForeignKey(x => x.StockId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Transactions)
                .WithOne(x => x.Stock)
                .HasForeignKey(x => x.StockId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* ===============================
           Inventory Item Configuration
        =============================== */
        modelBuilder.Entity<InventoryItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasIndex(x => x.SKU)
                .IsUnique();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Condition)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.HasOne(x => x.Stock)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.StockId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.AssignedTo)
                .WithMany()
                .HasForeignKey(x => x.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.InventoryCategory)
                .WithMany()
                .HasForeignKey(x => x.InventoryCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* ===============================
           Goods Receipt Configuration
        =============================== */
        modelBuilder.Entity<GoodsReceipt>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(gr => gr.PurchaseOrder)
                .WithMany()
                .HasForeignKey(gr => gr.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(gr => gr.ReceivedBy)
                .WithMany()
                .HasForeignKey(gr => gr.ReceivedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(gr => gr.InventoryProcessingStatus)
                .HasConversion<string>()
                .HasMaxLength(20);
        });

        modelBuilder.Entity<GoodsReceiptItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(gri => gri.GoodsReceipt)
                .WithMany(gr => gr.Items)
                .HasForeignKey(gri => gri.GoodsReceiptId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(gri => gri.PurchaseOrderItem)
                .WithMany()
                .HasForeignKey(gri => gri.PurchaseOrderItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(gri => gri.InventoryItem)
                .WithMany()
                .HasForeignKey(gri => gri.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* ===============================
           Inventory Assignment History Configuration
        =============================== */
        modelBuilder.Entity<InventoryAssignmentHistory>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(h => h.InventoryItem)
                .WithMany(i => i.AssignmentHistories)
                .HasForeignKey(h => h.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(h => h.AssignedTo)
                .WithMany()
                .HasForeignKey(h => h.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(h => h.PerformedBy)
                .WithMany()
                .HasForeignKey(h => h.PerformedById)
                .OnDelete(DeleteBehavior.Restrict);
        });

        /* ===============================
           Inventory Transaction Configuration
        =============================== */
        modelBuilder.Entity<InventoryTransaction>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.Stock)
                .WithMany(x => x.Transactions)
                .HasForeignKey(x => x.StockId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(x => x.StockId);
            entity.HasIndex(x => x.TransactionDate);
        });

        /* ===============================
           Inventory Request Configuration
        =============================== */
        modelBuilder.Entity<InventoryRequest>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.Purpose)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(x => x.Priority)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(x => x.Remarks)
                .HasMaxLength(1000);

            entity.HasOne(x => x.RequestedBy)
                .WithMany()
                .HasForeignKey(x => x.RequestedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Department)
                .WithMany()
                .HasForeignKey(x => x.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ApprovedByManager)
                .WithMany()
                .HasForeignKey(x => x.ApprovedByManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ProcessedByInventoryManager)
                .WithMany()
                .HasForeignKey(x => x.ProcessedByInventoryManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Items)
                .WithOne(x => x.InventoryRequest)
                .HasForeignKey(x => x.InventoryRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.RequestedById);
            entity.HasIndex(x => x.DepartmentId);
            entity.HasIndex(x => x.Status);
            entity.HasIndex(x => x.CreatedAt);
        });

        modelBuilder.Entity<InventoryRequestItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.Property(x => x.QuantityRequested)
                .IsRequired();

            entity.Property(x => x.QuantityIssued)
                .IsRequired();

            entity.HasOne(x => x.Stock)
                .WithMany()
                .HasForeignKey(x => x.StockId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.IssuedItems)
                .WithOne(x => x.InventoryRequestItem)
                .HasForeignKey(x => x.InventoryRequestItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.InventoryRequestId);
            entity.HasIndex(x => x.StockId);
        });

        modelBuilder.Entity<InventoryRequestIssuedItem>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.InventoryItem)
                .WithMany()
                .HasForeignKey(x => x.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.InventoryRequestItemId);
            entity.HasIndex(x => x.InventoryItemId);

            entity.HasIndex(x => new { x.InventoryRequestItemId, x.InventoryItemId })
                .IsUnique();
        });

        /* ===============================
           Seeds
        =============================== */
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