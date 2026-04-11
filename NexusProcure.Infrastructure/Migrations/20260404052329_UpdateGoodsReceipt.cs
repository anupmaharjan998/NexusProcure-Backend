using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGoodsReceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceiptItem_GoodsReceipts_GoodsReceiptId",
                table: "GoodsReceiptItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GoodsReceiptItem",
                table: "GoodsReceiptItem");

            migrationBuilder.RenameTable(
                name: "GoodsReceiptItem",
                newName: "GoodsReceiptItems");

            migrationBuilder.RenameIndex(
                name: "IX_GoodsReceiptItem_GoodsReceiptId",
                table: "GoodsReceiptItems",
                newName: "IX_GoodsReceiptItems_GoodsReceiptId");

            migrationBuilder.AddColumn<string>(
                name: "InventoryProcessingError",
                table: "GoodsReceipts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InventoryProcessingStatus",
                table: "GoodsReceipts",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "GoodsReceipts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReceivedById",
                table: "GoodsReceipts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "InventoryItemId",
                table: "GoodsReceiptItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "GoodsReceiptItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "InventoryInserted",
                table: "GoodsReceiptItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "InventoryInsertedAt",
                table: "GoodsReceiptItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "GoodsReceiptItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "GoodsReceiptItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PurchaseOrderItemId",
                table: "GoodsReceiptItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoodsReceiptItems",
                table: "GoodsReceiptItems",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_PurchaseOrderId",
                table: "GoodsReceipts",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceipts_ReceivedById",
                table: "GoodsReceipts",
                column: "ReceivedById");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_InventoryItemId",
                table: "GoodsReceiptItems",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItems_PurchaseOrderItemId",
                table: "GoodsReceiptItems",
                column: "PurchaseOrderItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceiptItems_GoodsReceipts_GoodsReceiptId",
                table: "GoodsReceiptItems",
                column: "GoodsReceiptId",
                principalTable: "GoodsReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceiptItems_InventoryItems_InventoryItemId",
                table: "GoodsReceiptItems",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceiptItems_PurchaseOrderItems_PurchaseOrderItemId",
                table: "GoodsReceiptItems",
                column: "PurchaseOrderItemId",
                principalTable: "PurchaseOrderItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceipts_PurchaseOrders_PurchaseOrderId",
                table: "GoodsReceipts",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceipts_Users_ReceivedById",
                table: "GoodsReceipts",
                column: "ReceivedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceiptItems_GoodsReceipts_GoodsReceiptId",
                table: "GoodsReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceiptItems_InventoryItems_InventoryItemId",
                table: "GoodsReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceiptItems_PurchaseOrderItems_PurchaseOrderItemId",
                table: "GoodsReceiptItems");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceipts_PurchaseOrders_PurchaseOrderId",
                table: "GoodsReceipts");

            migrationBuilder.DropForeignKey(
                name: "FK_GoodsReceipts_Users_ReceivedById",
                table: "GoodsReceipts");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceipts_PurchaseOrderId",
                table: "GoodsReceipts");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceipts_ReceivedById",
                table: "GoodsReceipts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GoodsReceiptItems",
                table: "GoodsReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceiptItems_InventoryItemId",
                table: "GoodsReceiptItems");

            migrationBuilder.DropIndex(
                name: "IX_GoodsReceiptItems_PurchaseOrderItemId",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "InventoryProcessingError",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "InventoryProcessingStatus",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "ReceivedById",
                table: "GoodsReceipts");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "InventoryInserted",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "InventoryInsertedAt",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "GoodsReceiptItems");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderItemId",
                table: "GoodsReceiptItems");

            migrationBuilder.RenameTable(
                name: "GoodsReceiptItems",
                newName: "GoodsReceiptItem");

            migrationBuilder.RenameIndex(
                name: "IX_GoodsReceiptItems_GoodsReceiptId",
                table: "GoodsReceiptItem",
                newName: "IX_GoodsReceiptItem_GoodsReceiptId");

            migrationBuilder.AlterColumn<Guid>(
                name: "InventoryItemId",
                table: "GoodsReceiptItem",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GoodsReceiptItem",
                table: "GoodsReceiptItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GoodsReceiptItem_GoodsReceipts_GoodsReceiptId",
                table: "GoodsReceiptItem",
                column: "GoodsReceiptId",
                principalTable: "GoodsReceipts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
