using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewFieldVendor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Vendors",
                newName: "BankName");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentTerms",
                table: "Vendors",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankBranch",
                table: "Vendors",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Vendors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxType",
                table: "Vendors",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CategoryId",
                table: "Vendors",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Categories_CategoryId",
                table: "Vendors",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Categories_CategoryId",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_CategoryId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "BankBranch",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "TaxType",
                table: "Vendors");

            migrationBuilder.RenameColumn(
                name: "BankName",
                table: "Vendors",
                newName: "Category");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentTerms",
                table: "Vendors",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);
        }
    }
}
