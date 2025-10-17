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
    public DbSet<InventoryItem> InventoryItems { get; set; }
    public DbSet<AssetAssignment> AssetAssignments { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Example: PR → PO 1-to-Many
        modelBuilder.Entity<PurchaseOrder>()
            .HasOne(po => po.Requisition)
            .WithMany(r => r.PurchaseOrders)
            .HasForeignKey(po => po.RequisitionId);

        // Example: User → Department 1-to-Many
        modelBuilder.Entity<User>()
            .HasOne(u => u.Department)
            .WithMany(d => d.Users)
            .HasForeignKey(u => u.DepartmentId);

        // Example: User → Role 1-to-Many
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId);
        
        modelBuilder.Entity<Department>()
            .HasOne(d => d.Head)
            .WithMany()
            .HasForeignKey(d => d.HeadId)
            .OnDelete(DeleteBehavior.Restrict); 
        
        // Seed Roles
        var adminRoleId = Guid.Parse("c76abcb8-63b5-4e14-8428-3a9a9b7ad001");
        var ceoRoleId = Guid.Parse("d27f6b43-9f64-4b13-a289-fd7744f2f102");
        var procurementRoleId = Guid.Parse("b38b2e23-6a7e-4c6d-9d5e-437a78c7b203");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "Admin" },
            new Role { Id = ceoRoleId, Name = "CEO" },
            new Role { Id = procurementRoleId, Name = "ProcurementOfficer" }
        );

        // Seed Admin User
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("a87f3d2b-0f0d-4b4e-9d2a-4e09d68f4104"),
                Username = "admin",
                Email = "admin@nexusprocure.com",
                PasswordHash = "AQAAAAIAAYagAAAAEHsHTY55ymmyC5FW7c6RpK2s/HWufLsNpUswO1iSjCFPadhi/WF+HZo86Twk4Rl4NQ==", // Admin@123
                RoleId = adminRoleId
            }
        );
        
        // Composite key for RolePermission
        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        // Relationships
        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId);
        
        
        PermissionSeed.Seed(modelBuilder);
    }
}