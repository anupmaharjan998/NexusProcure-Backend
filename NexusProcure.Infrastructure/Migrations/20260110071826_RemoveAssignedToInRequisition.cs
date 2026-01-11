using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAssignedToInRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Users_AssignedToUserId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_AssignedToUserId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Approvals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToUserId",
                table: "Approvals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_AssignedToUserId",
                table: "Approvals",
                column: "AssignedToUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Users_AssignedToUserId",
                table: "Approvals",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
