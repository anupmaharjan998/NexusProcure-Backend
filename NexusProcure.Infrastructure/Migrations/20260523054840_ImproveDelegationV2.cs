using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImproveDelegationV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Group", "Key" },
                values: new object[,]
                {
                    { new Guid("13000000-0000-0000-0000-000000000001"), "Manage all user delegations", "Delegation", "MANAGE_DELEGATION" },
                    { new Guid("13000000-0000-0000-0000-000000000002"), "Create and manage own delegation", "Delegation", "DELEGATION" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("13000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("13000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("13000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("13000000-0000-0000-0000-000000000002"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("13000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("13000000-0000-0000-0000-000000000002"));
        }
    }
}
