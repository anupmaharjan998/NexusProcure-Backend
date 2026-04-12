using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApprovalPolicyCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalPolicies_Category_CategoryId",
                table: "ApprovalPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Category_CategoryId",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_CategoryId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Vendors");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalPolicies_InventoryCategories_CategoryId",
                table: "ApprovalPolicies",
                column: "CategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalPolicies_InventoryCategories_CategoryId",
                table: "ApprovalPolicies");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Vendors",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RiskWeight = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CategoryId",
                table: "Vendors",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalPolicies_Category_CategoryId",
                table: "ApprovalPolicies",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Category_CategoryId",
                table: "Vendors",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }
    }
}
