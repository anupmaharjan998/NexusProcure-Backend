using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedManagerInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DelegateUserId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a87f3d2b-0f0d-4b4e-9d2a-4e09d68f4103"),
                columns: new[] { "DelegateUserId", "ManagerId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a87f3d2b-0f0d-4b4e-9d2a-4e09d68f4104"),
                columns: new[] { "DelegateUserId", "ManagerId" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Users_DelegateUserId",
                table: "Users",
                column: "DelegateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ManagerId",
                table: "Users",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_DelegateUserId",
                table: "Users",
                column: "DelegateUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_ManagerId",
                table: "Users",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_DelegateUserId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_ManagerId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DelegateUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ManagerId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DelegateUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "Users");
        }
    }
}
