using Microsoft.EntityFrameworkCore;
using NexusProcure.Core.Entities;

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
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111100");
        var ceoRoleId = Guid.Parse("11111111-1111-1111-1111-111111111101");
        var procurementRoleId = Guid.Parse("11111111-1111-1111-1111-111111111000");

        modelBuilder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "Admin" },
            new Role { Id = ceoRoleId, Name = "CEO" },
            new Role { Id = procurementRoleId, Name = "ProcurementOfficer" }
        );

        // Seed Admin User
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Username = "admin",
                Email = "admin@nexusprocure.com",
                PasswordHash = "$2a$11$y9F3ZBoHxlzE7x9/.R7KQ.9XasZDfWPGhKpD3gLE2/J6ZfCqzDq6a", // Admin@123
                RoleId = adminRoleId
            }
        );
    }
}