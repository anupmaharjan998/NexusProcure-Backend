using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Key" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "View all users", "VIEW_USERS" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Add new users", "CREATE_USER" },
                    { new Guid("10000000-0000-0000-0000-000000000003"), "Edit existing users", "EDIT_USER" },
                    { new Guid("10000000-0000-0000-0000-000000000004"), "Delete users", "DELETE_USER" },
                    { new Guid("20000000-0000-0000-0000-000000000001"), "View roles", "VIEW_ROLES" },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "Create new roles", "CREATE_ROLE" },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "Edit roles", "EDIT_ROLE" },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "Delete roles", "DELETE_ROLE" },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "Assign or revoke permissions for roles", "MANAGE_ROLE_PERMISSIONS" },
                    { new Guid("30000000-0000-0000-0000-000000000001"), "View departments", "VIEW_DEPARTMENTS" },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "Add new departments", "CREATE_DEPARTMENT" },
                    { new Guid("30000000-0000-0000-0000-000000000003"), "Edit departments", "EDIT_DEPARTMENT" },
                    { new Guid("30000000-0000-0000-0000-000000000004"), "Delete departments", "DELETE_DEPARTMENT" },
                    { new Guid("40000000-0000-0000-0000-000000000001"), "View vendor list", "VIEW_VENDORS" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "Add new vendor", "CREATE_VENDOR" },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "Edit vendor details", "EDIT_VENDOR" },
                    { new Guid("40000000-0000-0000-0000-000000000004"), "Delete vendor", "DELETE_VENDOR" },
                    { new Guid("50000000-0000-0000-0000-000000000001"), "Create purchase requisition", "CREATE_REQUISITION" },
                    { new Guid("50000000-0000-0000-0000-000000000002"), "Approve requisition", "APPROVE_REQUISITION" },
                    { new Guid("50000000-0000-0000-0000-000000000003"), "Create purchase order", "CREATE_PURCHASE_ORDER" },
                    { new Guid("50000000-0000-0000-0000-000000000004"), "View purchase order details", "VIEW_PURCHASE_ORDER" },
                    { new Guid("60000000-0000-0000-0000-000000000001"), "View inventory list", "VIEW_INVENTORY" },
                    { new Guid("60000000-0000-0000-0000-000000000002"), "Add new inventory items", "ADD_INVENTORY_ITEM" },
                    { new Guid("60000000-0000-0000-0000-000000000003"), "Update inventory items", "UPDATE_INVENTORY_ITEM" },
                    { new Guid("60000000-0000-0000-0000-000000000004"), "Delete inventory items", "DELETE_INVENTORY_ITEM" },
                    { new Guid("60000000-0000-0000-0000-000000000005"), "Assign asset to employee", "ASSIGN_ASSET" },
                    { new Guid("60000000-0000-0000-0000-000000000006"), "Return assigned asset", "RETURN_ASSET" },
                    { new Guid("70000000-0000-0000-0000-000000000001"), "View reports", "VIEW_REPORTS" },
                    { new Guid("70000000-0000-0000-0000-000000000002"), "Export reports to PDF/Excel", "EXPORT_REPORTS" },
                    { new Guid("80000000-0000-0000-0000-000000000001"), "Change global settings", "MANAGE_SYSTEM_SETTINGS" },
                    { new Guid("80000000-0000-0000-0000-000000000002"), "View audit logs", "VIEW_AUDIT_LOGS" },
                    { new Guid("80000000-0000-0000-0000-000000000003"), "Configure approval workflow", "MANAGE_APPROVAL_WORKFLOW" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a87f3d2b-0f0d-4b4e-9d2a-4e09d68f4104"),
                column: "IsActive",
                value: false);

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Users");
        }
    }
}
