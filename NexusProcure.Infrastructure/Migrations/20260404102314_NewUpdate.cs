using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000006"),
                columns: new[] { "Description", "Key" },
                values: new object[] { "Unassigned asset from employee", "UNASSIGN_ASSET" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Group", "Key" },
                values: new object[,]
                {
                    { new Guid("11000000-0000-0000-0000-000000000002"), "Update permissions", "Permissions", "UPDATE_PERMISSIONS" },
                    { new Guid("12000000-0000-0000-0000-000000000001"), "Add permissions", "Policies", "ADD_POLICIES" },
                    { new Guid("12000000-0000-0000-0000-000000000002"), "Delete permissions", "Policies", "DELETE_POLICIES" },
                    { new Guid("12000000-0000-0000-0000-000000000003"), "Update permissions", "Policies", "UPDATE_POLICIES" },
                    { new Guid("12000000-0000-0000-0000-000000000004"), "Add total amount risk score", "Policies", "ADD_TOTAL_AMOUNT_RISK_SCORE" },
                    { new Guid("12000000-0000-0000-0000-000000000005"), "Update total amount risk score", "Policies", "UPDATE_TOTAL_AMOUNT_RISK_SCORE" },
                    { new Guid("12000000-0000-0000-0000-000000000006"), "Delete total amount risk score", "Policies", "DELETE_TOTAL_AMOUNT_RISK_SCORE" },
                    { new Guid("90000000-0000-0000-0000-000000000002"), "Add new category", "Category", "ADD_CATEGORIES" },
                    { new Guid("90000000-0000-0000-0000-000000000003"), "Update category", "Category", "UPDATE_CATEGORIES" },
                    { new Guid("90000000-0000-0000-0000-000000000004"), "Delete category", "Category", "DELETE_CATEGORIES" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("11000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("12000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("12000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("12000000-0000-0000-0000-000000000003"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("12000000-0000-0000-0000-000000000004"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("12000000-0000-0000-0000-000000000005"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("12000000-0000-0000-0000-000000000006"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("90000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("90000000-0000-0000-0000-000000000003"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("90000000-0000-0000-0000-000000000004"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("11000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("12000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("12000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("12000000-0000-0000-0000-000000000003"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("12000000-0000-0000-0000-000000000004"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("12000000-0000-0000-0000-000000000005"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("12000000-0000-0000-0000-000000000006"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("90000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("90000000-0000-0000-0000-000000000003"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("90000000-0000-0000-0000-000000000004"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("11000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("12000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("12000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("12000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("12000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("12000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("12000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000004"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("60000000-0000-0000-0000-000000000006"),
                columns: new[] { "Description", "Key" },
                values: new object[] { "Return assigned asset", "RETURN_ASSET" });
        }
    }
}
