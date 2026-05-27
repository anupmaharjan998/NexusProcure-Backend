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
                new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Group = "User", Key = "VIEW_USERS", Description = "View all users" },
                new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Group = "User", Key = "CREATE_USER", Description = "Add new users" },
                new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Group = "User", Key = "EDIT_USER", Description = "Edit existing users" },
                new Permission { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Group = "User", Key = "DELETE_USER", Description = "Delete users" },

                // ------------------ Role Management ------------------
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Group = "Role", Key = "VIEW_ROLES", Description = "View roles" },
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Group = "Role", Key = "CREATE_ROLE", Description = "Create new roles" },
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Group = "Role", Key = "EDIT_ROLE", Description = "Edit roles" },
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000004"), Group = "Role", Key = "DELETE_ROLE", Description = "Delete roles" },
                new Permission { Id = Guid.Parse("20000000-0000-0000-0000-000000000005"), Group = "Role", Key = "MANAGE_ROLE_PERMISSIONS", Description = "Assign or revoke permissions for roles" },

                // ------------------ Department Management ------------------
                new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), Group = "Department", Key = "VIEW_DEPARTMENTS", Description = "View departments" },
                new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), Group = "Department", Key = "CREATE_DEPARTMENT", Description = "Add new departments" },
                new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), Group = "Department", Key = "EDIT_DEPARTMENT", Description = "Edit departments" },
                new Permission { Id = Guid.Parse("30000000-0000-0000-0000-000000000004"), Group = "Department", Key = "DELETE_DEPARTMENT", Description = "Delete departments" },

                // ------------------ Vendor Management ------------------
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000001"), Group = "Vendor", Key = "ADD_VENDOR", Description = "Create a new vendor" },
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000002"), Group = "Vendor", Key = "EDIT_VENDOR", Description = "Edit vendor details" },
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000003"), Group = "Vendor", Key = "VIEW_VENDOR", Description = "View vendor records" },
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000004"), Group = "Vendor", Key = "APPROVE_VENDOR", Description = "Approve or reject vendor applications" },
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000005"), Group = "Vendor", Key = "DELETE_VENDOR", Description = "Delete vendor records" },
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000006"), Group = "Vendor", Key = "UPLOAD_VENDOR_DOCUMENT", Description = "Upload vendor documents" },
                new Permission { Id = Guid.Parse("40000000-0000-0000-0000-000000000007"), Group = "Vendor", Key = "DELETE_VENDOR_DOCUMENT", Description = "Delete vendor document" },

                // ------------------ Procurement ------------------
                new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000001"), Group = "Procurement", Key = "CREATE_REQUISITION", Description = "Create purchase requisition" },
                new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000002"), Group = "Procurement", Key = "APPROVE_REQUISITION", Description = "Approve requisition" },
                new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000003"), Group = "Procurement", Key = "CREATE_PURCHASE_ORDER", Description = "Create purchase order" },
                new Permission { Id = Guid.Parse("50000000-0000-0000-0000-000000000004"), Group = "Procurement", Key = "VIEW_PURCHASE_ORDER", Description = "View purchase order details" },

                // ------------------ Inventory & Asset Management ------------------
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000001"), Group = "Inventory", Key = "VIEW_INVENTORY", Description = "View inventory list" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000002"), Group = "Inventory", Key = "ADD_INVENTORY_ITEM", Description = "Add new inventory items" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000003"), Group = "Inventory", Key = "UPDATE_INVENTORY_ITEM", Description = "Update inventory items" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000004"), Group = "Inventory", Key = "DELETE_INVENTORY_ITEM", Description = "Delete inventory items" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000005"), Group = "Inventory", Key = "ASSIGN_ASSET", Description = "Assign asset to employee" },
                new Permission { Id = Guid.Parse("60000000-0000-0000-0000-000000000006"), Group = "Inventory", Key = "UNASSIGN_ASSET", Description = "Unassigned asset from employee" },

                // ------------------ Reporting ------------------
                new Permission { Id = Guid.Parse("70000000-0000-0000-0000-000000000001"), Group = "Reporting", Key = "VIEW_REPORTS", Description = "View reports" },
                new Permission { Id = Guid.Parse("70000000-0000-0000-0000-000000000002"), Group = "Reporting", Key = "EXPORT_REPORTS", Description = "Export reports to PDF/Excel" },

                // ------------------ System & Admin ------------------
                new Permission { Id = Guid.Parse("80000000-0000-0000-0000-000000000001"), Group = "System", Key = "MANAGE_SYSTEM_SETTINGS", Description = "Change global settings" },
                new Permission { Id = Guid.Parse("80000000-0000-0000-0000-000000000002"), Group = "System", Key = "VIEW_AUDIT_LOGS", Description = "View audit logs" },
                new Permission { Id = Guid.Parse("80000000-0000-0000-0000-000000000003"), Group = "System", Key = "MANAGE_APPROVAL_WORKFLOW", Description = "Configure approval workflow" },
                
                // ------------------ System & Admin ------------------
                new Permission { Id = Guid.Parse("90000000-0000-0000-0000-000000000001"), Group = "Category", Key = "VIEW_CATEGORIES", Description = "View category" },
                new Permission { Id = Guid.Parse("90000000-0000-0000-0000-000000000002"), Group = "Category", Key = "ADD_CATEGORIES", Description = "Add new category" },
                new Permission { Id = Guid.Parse("90000000-0000-0000-0000-000000000003"), Group = "Category", Key = "UPDATE_CATEGORIES", Description = "Update category" },
                new Permission { Id = Guid.Parse("90000000-0000-0000-0000-000000000004"), Group = "Category", Key = "DELETE_CATEGORIES", Description = "Delete category" },
                
                // ------------------ Permissions ------------------
                new Permission { Id = Guid.Parse("11000000-0000-0000-0000-000000000001"), Group = "Permissions", Key = "VIEW_PERMISSIONS", Description = "View permissions" },
                new Permission { Id = Guid.Parse("11000000-0000-0000-0000-000000000002"), Group = "Permissions", Key = "UPDATE_PERMISSIONS", Description = "Update permissions" },
                
                // ------------------ Policy ------------------
                new Permission { Id = Guid.Parse("12000000-0000-0000-0000-000000000001"), Group = "Policies", Key = "ADD_POLICIES", Description = "Add permissions" },
                new Permission { Id = Guid.Parse("12000000-0000-0000-0000-000000000002"), Group = "Policies", Key = "DELETE_POLICIES", Description = "Delete permissions" },
                new Permission { Id = Guid.Parse("12000000-0000-0000-0000-000000000003"), Group = "Policies", Key = "UPDATE_POLICIES", Description = "Update permissions" },
                new Permission { Id = Guid.Parse("12000000-0000-0000-0000-000000000004"), Group = "Policies", Key = "ADD_TOTAL_AMOUNT_RISK_SCORE", Description = "Add total amount risk score" },
                new Permission { Id = Guid.Parse("12000000-0000-0000-0000-000000000005"), Group = "Policies", Key = "UPDATE_TOTAL_AMOUNT_RISK_SCORE", Description = "Update total amount risk score" },
                new Permission { Id = Guid.Parse("12000000-0000-0000-0000-000000000006"), Group = "Policies", Key = "DELETE_TOTAL_AMOUNT_RISK_SCORE", Description = "Delete total amount risk score" },
                
                // ------------------ Delegation ------------------
                new Permission { Id = Guid.Parse("13000000-0000-0000-0000-000000000001"), Group = "Delegation", Key = "MANAGE_DELEGATION", Description = "Manage all user delegations" },
                new Permission { Id = Guid.Parse("13000000-0000-0000-0000-000000000002"), Group = "Delegation", Key = "DELEGATION", Description = "Create and manage own delegation" },
                
                
                // ------------------ Dashboard ------------------
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000001"), Group = "Dashboard", Key = "VIEW_DASHBOARD", Description = "View dashboard" },

                // Employee dashboard
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000002"), Group = "Dashboard", Key = "VIEW_EMPLOYEE_DASHBOARD", Description = "View employee dashboard summary" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000003"), Group = "Dashboard", Key = "VIEW_MY_REQUISITION_STATS", Description = "View own requisition statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000004"), Group = "Dashboard", Key = "VIEW_MY_ASSIGNED_ITEMS", Description = "View own assigned inventory items" },

                // Manager dashboard
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000005"), Group = "Dashboard", Key = "VIEW_MANAGER_DASHBOARD", Description = "View manager dashboard summary" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000006"), Group = "Dashboard", Key = "VIEW_DEPARTMENT_REQUISITION_STATS", Description = "View department requisition statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000007"), Group = "Dashboard", Key = "VIEW_PENDING_APPROVAL_STATS", Description = "View pending approval statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000008"), Group = "Dashboard", Key = "VIEW_DEPARTMENT_INVENTORY_STATS", Description = "View department inventory statistics" },

                // Procurement dashboard
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000009"), Group = "Dashboard", Key = "VIEW_PROCUREMENT_DASHBOARD", Description = "View procurement dashboard summary" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000010"), Group = "Dashboard", Key = "VIEW_PROCUREMENT_QUEUE_STATS", Description = "View approved requisitions waiting for procurement" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000011"), Group = "Dashboard", Key = "VIEW_RFQ_STATS", Description = "View RFQ statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000012"), Group = "Dashboard", Key = "VIEW_QUOTATION_STATS", Description = "View quotation statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000013"), Group = "Dashboard", Key = "VIEW_PURCHASE_ORDER_STATS", Description = "View purchase order statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000014"), Group = "Dashboard", Key = "VIEW_RECENT_PURCHASE_ORDERS", Description = "View recent purchase orders on dashboard" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000015"), Group = "Dashboard", Key = "VIEW_TODAY_DELIVERIES", Description = "View today purchase order deliveries" },

                // Inventory dashboard
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000016"), Group = "Dashboard", Key = "VIEW_INVENTORY_DASHBOARD", Description = "View inventory dashboard summary" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000017"), Group = "Dashboard", Key = "VIEW_STOCK_STATS", Description = "View stock statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000018"), Group = "Dashboard", Key = "VIEW_LOW_STOCK_ALERTS", Description = "View low stock alerts" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000019"), Group = "Dashboard", Key = "VIEW_INVENTORY_ASSIGNMENT_STATS", Description = "View inventory assignment statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000020"), Group = "Dashboard", Key = "VIEW_RECEIVING_STATS", Description = "View purchase order receiving statistics" },

                // Finance dashboard
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000021"), Group = "Dashboard", Key = "VIEW_FINANCE_DASHBOARD", Description = "View finance dashboard summary" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000022"), Group = "Dashboard", Key = "VIEW_PURCHASE_COST_STATS", Description = "View purchase cost statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000023"), Group = "Dashboard", Key = "VIEW_BUDGET_STATS", Description = "View budget and department-wise procurement cost statistics" },

                // CEO / Executive dashboard
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000024"), Group = "Dashboard", Key = "VIEW_EXECUTIVE_DASHBOARD", Description = "View executive dashboard summary" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000025"), Group = "Dashboard", Key = "VIEW_EXECUTIVE_PROCUREMENT_STATS", Description = "View executive procurement statistics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000026"), Group = "Dashboard", Key = "VIEW_DASHBOARD_CHARTS", Description = "View dashboard charts and analytics" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000027"), Group = "Dashboard", Key = "VIEW_DASHBOARD_ALERTS", Description = "View dashboard alerts and risk indicators" },

                // Admin dashboard
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000028"), Group = "Dashboard", Key = "VIEW_ADMIN_DASHBOARD", Description = "View admin dashboard summary" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000029"), Group = "Dashboard", Key = "VIEW_SYSTEM_STATS", Description = "View system statistics including users, roles, permissions and departments" },

                // Dashboard reports / actions
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000030"), Group = "Dashboard", Key = "VIEW_DASHBOARD_REPORTS", Description = "View dashboard reports" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000031"), Group = "Dashboard", Key = "EXPORT_DASHBOARD_REPORTS", Description = "Export dashboard reports" },
                new Permission { Id = Guid.Parse("14000000-0000-0000-0000-000000000032"), Group = "Dashboard", Key = "VIEW_DASHBOARD_QUICK_ACTIONS", Description = "View dashboard quick actions" },

            };

            modelBuilder.Entity<Permission>().HasData(permissions);

            // Ensure Admin role has all permissions by default
            var adminRoleId = Guid.Parse("c76abcb8-63b5-4e14-8428-3a9a9b7ad001");
            var adminRolePermissions = permissions.Select(p => new RolePermission
            {
                RoleId = adminRoleId,
                PermissionId = p.Id
            }).ToList();

            modelBuilder.Entity<RolePermission>().HasData(adminRolePermissions);
        }
    }
}
