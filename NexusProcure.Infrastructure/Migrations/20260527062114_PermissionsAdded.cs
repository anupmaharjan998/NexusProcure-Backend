using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PermissionsAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Group", "Key" },
                values: new object[,]
                {
                    { new Guid("14000000-0000-0000-0000-000000000001"), "View dashboard", "Dashboard", "VIEW_DASHBOARD" },
                    { new Guid("14000000-0000-0000-0000-000000000002"), "View employee dashboard summary", "Dashboard", "VIEW_EMPLOYEE_DASHBOARD" },
                    { new Guid("14000000-0000-0000-0000-000000000003"), "View own requisition statistics", "Dashboard", "VIEW_MY_REQUISITION_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000004"), "View own assigned inventory items", "Dashboard", "VIEW_MY_ASSIGNED_ITEMS" },
                    { new Guid("14000000-0000-0000-0000-000000000005"), "View manager dashboard summary", "Dashboard", "VIEW_MANAGER_DASHBOARD" },
                    { new Guid("14000000-0000-0000-0000-000000000006"), "View department requisition statistics", "Dashboard", "VIEW_DEPARTMENT_REQUISITION_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000007"), "View pending approval statistics", "Dashboard", "VIEW_PENDING_APPROVAL_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000008"), "View department inventory statistics", "Dashboard", "VIEW_DEPARTMENT_INVENTORY_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000009"), "View procurement dashboard summary", "Dashboard", "VIEW_PROCUREMENT_DASHBOARD" },
                    { new Guid("14000000-0000-0000-0000-000000000010"), "View approved requisitions waiting for procurement", "Dashboard", "VIEW_PROCUREMENT_QUEUE_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000011"), "View RFQ statistics", "Dashboard", "VIEW_RFQ_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000012"), "View quotation statistics", "Dashboard", "VIEW_QUOTATION_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000013"), "View purchase order statistics", "Dashboard", "VIEW_PURCHASE_ORDER_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000014"), "View recent purchase orders on dashboard", "Dashboard", "VIEW_RECENT_PURCHASE_ORDERS" },
                    { new Guid("14000000-0000-0000-0000-000000000015"), "View today purchase order deliveries", "Dashboard", "VIEW_TODAY_DELIVERIES" },
                    { new Guid("14000000-0000-0000-0000-000000000016"), "View inventory dashboard summary", "Dashboard", "VIEW_INVENTORY_DASHBOARD" },
                    { new Guid("14000000-0000-0000-0000-000000000017"), "View stock statistics", "Dashboard", "VIEW_STOCK_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000018"), "View low stock alerts", "Dashboard", "VIEW_LOW_STOCK_ALERTS" },
                    { new Guid("14000000-0000-0000-0000-000000000019"), "View inventory assignment statistics", "Dashboard", "VIEW_INVENTORY_ASSIGNMENT_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000020"), "View purchase order receiving statistics", "Dashboard", "VIEW_RECEIVING_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000021"), "View finance dashboard summary", "Dashboard", "VIEW_FINANCE_DASHBOARD" },
                    { new Guid("14000000-0000-0000-0000-000000000022"), "View purchase cost statistics", "Dashboard", "VIEW_PURCHASE_COST_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000023"), "View budget and department-wise procurement cost statistics", "Dashboard", "VIEW_BUDGET_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000024"), "View executive dashboard summary", "Dashboard", "VIEW_EXECUTIVE_DASHBOARD" },
                    { new Guid("14000000-0000-0000-0000-000000000025"), "View executive procurement statistics", "Dashboard", "VIEW_EXECUTIVE_PROCUREMENT_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000026"), "View dashboard charts and analytics", "Dashboard", "VIEW_DASHBOARD_CHARTS" },
                    { new Guid("14000000-0000-0000-0000-000000000027"), "View dashboard alerts and risk indicators", "Dashboard", "VIEW_DASHBOARD_ALERTS" },
                    { new Guid("14000000-0000-0000-0000-000000000028"), "View admin dashboard summary", "Dashboard", "VIEW_ADMIN_DASHBOARD" },
                    { new Guid("14000000-0000-0000-0000-000000000029"), "View system statistics including users, roles, permissions and departments", "Dashboard", "VIEW_SYSTEM_STATS" },
                    { new Guid("14000000-0000-0000-0000-000000000030"), "View dashboard reports", "Dashboard", "VIEW_DASHBOARD_REPORTS" },
                    { new Guid("14000000-0000-0000-0000-000000000031"), "Export dashboard reports", "Dashboard", "EXPORT_DASHBOARD_REPORTS" },
                    { new Guid("14000000-0000-0000-0000-000000000032"), "View dashboard quick actions", "Dashboard", "VIEW_DASHBOARD_QUICK_ACTIONS" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("14000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000003"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000004"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000005"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000006"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000007"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000008"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000009"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000010"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000011"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000012"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000013"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000014"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000015"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000016"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000017"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000018"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000019"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000020"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000021"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000022"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000023"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000024"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000025"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000026"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000027"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000028"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000029"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000030"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000031"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("14000000-0000-0000-0000-000000000032"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000003"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000004"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000005"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000006"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000007"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000008"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000009"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000010"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000011"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000012"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000013"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000014"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000015"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000016"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000017"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000018"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000019"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000020"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000021"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000022"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000023"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000024"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000025"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000026"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000027"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000028"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000029"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000030"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000031"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("14000000-0000-0000-0000-000000000032"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000016"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000017"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000018"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000019"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000020"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000021"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000022"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000023"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000024"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000025"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000026"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000027"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000028"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000029"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000030"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000031"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("14000000-0000-0000-0000-000000000032"));
        }
    }
}
