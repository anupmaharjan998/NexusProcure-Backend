using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImprovedRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Requisitions_RequisitionId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Requisitions_RequisitionId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_InventoryCategories_CategoryId",
                table: "Requisitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Users_RequestedById",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_CategoryId",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_RequisitionId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "ItemName",
                table: "RequisitionItems");

            migrationBuilder.DropColumn(
                name: "RequisitionId",
                table: "Approvals");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Requisitions",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Requisitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "RequisitionNumber",
                table: "Requisitions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Requisitions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "Requisitions",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequiredDate",
                table: "Requisitions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Requisitions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedCost",
                table: "RequisitionItems",
                type: "numeric(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryStockId",
                table: "RequisitionItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "RequisitionItems",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_UserId",
                table: "Requisitions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequisitionItems_InventoryStockId",
                table: "RequisitionItems",
                column: "InventoryStockId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ReferenceId",
                table: "Approvals",
                column: "ReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Requisitions_ReferenceId",
                table: "Approvals",
                column: "ReferenceId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Requisitions_RequisitionId",
                table: "PurchaseOrders",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RequisitionItems_InventoryStocks_InventoryStockId",
                table: "RequisitionItems",
                column: "InventoryStockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Users_RequestedById",
                table: "Requisitions",
                column: "RequestedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Users_UserId",
                table: "Requisitions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Requisitions_ReferenceId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Requisitions_RequisitionId",
                table: "PurchaseOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_RequisitionItems_InventoryStocks_InventoryStockId",
                table: "RequisitionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Users_RequestedById",
                table: "Requisitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Users_UserId",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_UserId",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_RequisitionItems_InventoryStockId",
                table: "RequisitionItems");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_ReferenceId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "RequiredDate",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "InventoryStockId",
                table: "RequisitionItems");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "RequisitionItems");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Requisitions",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Requisitions",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "RequisitionNumber",
                table: "Requisitions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Requisitions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<decimal>(
                name: "EstimatedCost",
                table: "RequisitionItems",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "ItemName",
                table: "RequisitionItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RequisitionId",
                table: "Approvals",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_CategoryId",
                table: "Requisitions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_RequisitionId",
                table: "Approvals",
                column: "RequisitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Requisitions_RequisitionId",
                table: "Approvals",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Requisitions_RequisitionId",
                table: "PurchaseOrders",
                column: "RequisitionId",
                principalTable: "Requisitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_InventoryCategories_CategoryId",
                table: "Requisitions",
                column: "CategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Users_RequestedById",
                table: "Requisitions",
                column: "RequestedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
