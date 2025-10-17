using Microsoft.EntityFrameworkCore;
using NexusProcure.Core.Entities;

namespace NexusProcure.Infrastructure.Data.Seeds
{
    public static class PermissionSeed
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            var permissions = new List<Permission>
            {
                // ------------------ User Management ------------------
                new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Key = "VIEW_USERS", Description = "View all users" },
                new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Key = "CREATE_USER", Description = "Add new users" },
                new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Key = "EDIT_USER", Description = "Edit existing users" },
                new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Key = "DELETE_USER", Description = "Delete users" },

                // ------------------ Role Management ------------------
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Key = "VIEW_ROLES", Description = "View roles" },
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Key = "CREATE_ROLE", Description = "Create new roles" },
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Key = "EDIT_ROLE", Description = "Edit roles" },
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Key = "DELETE_ROLE", Description = "Delete roles" },
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Key = "MANAGE_ROLE_PERMISSIONS", Description = "Assign or revoke permissions for roles" },

                // ------------------ Department Management ------------------
                new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), Key = "VIEW_DEPARTMENTS", Description = "View departments" },
                new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), Key = "CREATE_DEPARTMENT", Description = "Add new departments" },
                new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), Key = "EDIT_DEPARTMENT", Description = "Edit departments" },
                new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000004"), Key = "DELETE_DEPARTMENT", Description = "Delete departments" },

                // ------------------ Vendor Management ------------------
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), Key = "VIEW_VENDORS", Description = "View vendor list" },
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), Key = "CREATE_VENDOR", Description = "Add new vendor" },
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), Key = "EDIT_VENDOR", Description = "Edit vendor details" },
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), Key = "DELETE_VENDOR", Description = "Delete vendor" },

                // ------------------ Procurement ------------------
                new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), Key = "CREATE_REQUISITION", Description = "Create purchase requisition" },
                new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), Key = "APPROVE_REQUISITION", Description = "Approve requisition" },
                new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000003"), Key = "CREATE_PURCHASE_ORDER", Description = "Create purchase order" },
                new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000004"), Key = "VIEW_PURCHASE_ORDER", Description = "View purchase order details" },

                // ------------------ Inventory & Asset Management ------------------
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), Key = "VIEW_INVENTORY", Description = "View inventory list" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000002"), Key = "ADD_INVENTORY_ITEM", Description = "Add new inventory items" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000003"), Key = "UPDATE_INVENTORY_ITEM", Description = "Update inventory items" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000004"), Key = "DELETE_INVENTORY_ITEM", Description = "Delete inventory items" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000005"), Key = "ASSIGN_ASSET", Description = "Assign asset to employee" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000006"), Key = "RETURN_ASSET", Description = "Return assigned asset" },

                // ------------------ Reporting ------------------
                new Permission { Id = Guid.Parse("70000000-0000-0000-0000-000000000001"), Key = "VIEW_REPORTS", Description = "View reports" },
                new Permission { Id = Guid.Parse("70000000-0000-0000-0000-000000000002"), Key = "EXPORT_REPORTS", Description = "Export reports to PDF/Excel" },

                // ------------------ System & Admin ------------------
                new Permission { Id = Guid.Parse("80000000-0000-0000-0000-000000000001"), Key = "MANAGE_SYSTEM_SETTINGS", Description = "Change global settings" },
                new Permission { Id = Guid.Parse("80000000-0000-0000-0000-000000000002"), Key = "VIEW_AUDIT_LOGS", Description = "View audit logs" },
                new Permission { Id = Guid.Parse("80000000-0000-0000-0000-000000000003"), Key = "MANAGE_APPROVAL_WORKFLOW", Description = "Configure approval workflow" },
            };

            modelBuilder.Entity<Permission>().HasData(permissions);
        }
    }
}
