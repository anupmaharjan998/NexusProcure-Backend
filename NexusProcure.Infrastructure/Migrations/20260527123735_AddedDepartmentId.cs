using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDepartmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcurementRequests_Users_RequestedByUserId",
                table: "ProcurementRequests");

            migrationBuilder.RenameColumn(
                name: "RequestedByUserId",
                table: "ProcurementRequests",
                newName: "RequestedById");

            migrationBuilder.RenameColumn(
                name: "ManagerRemarks",
                table: "ProcurementRequests",
                newName: "Remarks");

            migrationBuilder.RenameIndex(
                name: "IX_ProcurementRequests_RequestedByUserId",
                table: "ProcurementRequests",
                newName: "IX_ProcurementRequests_RequestedById");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "ProcurementRequests",
                type: "integer",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "ProcurementRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "ProcurementRequests",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequisitionId",
                table: "ProcurementRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ProcurementRequests",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ProcurementRequestItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementRequests_DepartmentId",
                table: "ProcurementRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementRequests_RequisitionId",
                table: "ProcurementRequests",
                column: "RequisitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcurementRequests_Departments_DepartmentId",
                table: "ProcurementRequests",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcurementRequests_Requisitions_RequisitionId",
                table: "ProcurementRequests",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcurementRequests_Users_RequestedById",
                table: "ProcurementRequests",
                column: "RequestedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcurementRequests_Departments_DepartmentId",
                table: "ProcurementRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcurementRequests_Requisitions_RequisitionId",
                table: "ProcurementRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ProcurementRequests_Users_RequestedById",
                table: "ProcurementRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProcurementRequests_DepartmentId",
                table: "ProcurementRequests");

            migrationBuilder.DropIndex(
                name: "IX_ProcurementRequests_RequisitionId",
                table: "ProcurementRequests");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "ProcurementRequests");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "ProcurementRequests");

            migrationBuilder.DropColumn(
                name: "RequisitionId",
                table: "ProcurementRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ProcurementRequests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ProcurementRequestItems");

            migrationBuilder.RenameColumn(
                name: "RequestedById",
                table: "ProcurementRequests",
                newName: "RequestedByUserId");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "ProcurementRequests",
                newName: "ManagerRemarks");

            migrationBuilder.RenameIndex(
                name: "IX_ProcurementRequests_RequestedById",
                table: "ProcurementRequests",
                newName: "IX_ProcurementRequests_RequestedByUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProcurementRequests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_ProcurementRequests_Users_RequestedByUserId",
                table: "ProcurementRequests",
                column: "RequestedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
