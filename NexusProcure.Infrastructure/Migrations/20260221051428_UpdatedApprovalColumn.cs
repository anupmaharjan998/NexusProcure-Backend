using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedApprovalColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Requisitions_RequisitionId",
                table: "Approvals");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequisitionId",
                table: "Approvals",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceId",
                table: "Approvals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "ReferenceType",
                table: "Approvals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Requisitions_RequisitionId",
                table: "Approvals",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Requisitions_RequisitionId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "Approvals");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequisitionId",
                table: "Approvals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Requisitions_RequisitionId",
                table: "Approvals",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
