using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVendortable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Category_CategoryId1",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_InventoryCategories_CategoryId",
                table: "Vendors");

            migrationBuilder.RenameColumn(
                name: "CategoryId1",
                table: "Vendors",
                newName: "InventoryCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Vendors_CategoryId1",
                table: "Vendors",
                newName: "IX_Vendors_InventoryCategoryId");

            migrationBuilder.CreateTable(
                name: "VendorCategory",
                columns: table => new
                {
                    VendorId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCategory", x => new { x.VendorId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_VendorCategory_InventoryCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InventoryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendorCategory_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorCategory_CategoryId",
                table: "VendorCategory",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Category_CategoryId",
                table: "Vendors",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_InventoryCategories_InventoryCategoryId",
                table: "Vendors",
                column: "InventoryCategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Category_CategoryId",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_InventoryCategories_InventoryCategoryId",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "VendorCategory");

            migrationBuilder.RenameColumn(
                name: "InventoryCategoryId",
                table: "Vendors",
                newName: "CategoryId1");

            migrationBuilder.RenameIndex(
                name: "IX_Vendors_InventoryCategoryId",
                table: "Vendors",
                newName: "IX_Vendors_CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Category_CategoryId1",
                table: "Vendors",
                column: "CategoryId1",
                principalTable: "Category",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_InventoryCategories_CategoryId",
                table: "Vendors",
                column: "CategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
