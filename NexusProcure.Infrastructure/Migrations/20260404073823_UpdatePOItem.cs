using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePOItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryCategoryId",
                table: "PurchaseOrderItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_InventoryCategoryId",
                table: "PurchaseOrderItems",
                column: "InventoryCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_InventoryCategories_InventoryCategoryId",
                table: "PurchaseOrderItems",
                column: "InventoryCategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_InventoryCategories_InventoryCategoryId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItems_InventoryCategoryId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "InventoryCategoryId",
                table: "PurchaseOrderItems");
        }
    }
}
