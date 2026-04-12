using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequisitionCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Category_CategoryId",
                table: "Requisitions");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_InventoryCategories_CategoryId",
                table: "Requisitions",
                column: "CategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_InventoryCategories_CategoryId",
                table: "Requisitions");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Category_CategoryId",
                table: "Requisitions",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
