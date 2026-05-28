using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "Description",
                value: "Create purchase requisition and RFQ");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "Description",
                value: "Approve requisition and Quotation");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Group", "Key" },
                values: new object[,]
                {
                    { new Guid("12000000-0000-0000-0000-000000000007"), "View permissions", "Policies", "VIEW_POLICIES" },
                    { new Guid("50000000-0000-0000-0000-000000000005"), "Receive purchase order details", "Procurement", "RECEIVE_PURCHASE_ORDER" },
                    { new Guid("60000000-0000-0000-0000-000000000007"), "Request asset from inventory", "Inventory", "REQUEST_ASSET" },
                    { new Guid("60000000-0000-0000-0000-000000000008"), "Approve  asset request", "Inventory", "REQUEST_ASSET_APPROVAL" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("12000000-0000-0000-0000-000000000007"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("50000000-0000-0000-0000-000000000005"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("60000000-0000-0000-0000-000000000007"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("60000000-0000-0000-0000-000000000008"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("12000000-0000-0000-0000-000000000007"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("50000000-0000-0000-0000-000000000005"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("60000000-0000-0000-0000-000000000007"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("60000000-0000-0000-0000-000000000008"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("12000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000008"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000001"),
                column: "Description",
                value: "Create purchase requisition");

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("50000000-0000-0000-0000-000000000002"),
                column: "Description",
                value: "Approve requisition");
        }
    }
}
