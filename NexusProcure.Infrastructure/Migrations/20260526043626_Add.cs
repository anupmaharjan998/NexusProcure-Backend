using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InventoryCategoryId",
                table: "QuotationItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationItems_InventoryCategoryId",
                table: "QuotationItems",
                column: "InventoryCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuotationItems_InventoryCategories_InventoryCategoryId",
                table: "QuotationItems",
                column: "InventoryCategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuotationItems_InventoryCategories_InventoryCategoryId",
                table: "QuotationItems");

            migrationBuilder.DropIndex(
                name: "IX_QuotationItems_InventoryCategoryId",
                table: "QuotationItems");

            migrationBuilder.DropColumn(
                name: "InventoryCategoryId",
                table: "QuotationItems");
        }
    }
}
