using Microsoft.EntityFrameworkCore;
using NexusProcure.Core.Entities;
using NexusProcure.Infrastructure.Data.Seeds;

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

    public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
    public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

    public DbSet<Vendor> Vendors { get; set; }
    public DbSet<VendorDocument> VendorDocuments { get; set; }

    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<AssetAssignment> AssetAssignments { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Requisition → Items (1-M)
        modelBuilder.Entity<Requisition>()
            .HasMany(r => r.Items)
            .WithOne(i => i.Requisition)
            .HasForeignKey(i => i.RequisitionId)
            .OnDelete(DeleteBehavior.Cascade);

        
        // PR → PO (1-M)
        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(po => po.Requisition)
            .WithMany(r => r.PurchaseOrders)
            .HasForeignKey(po => po.RequisitionId);

        
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
