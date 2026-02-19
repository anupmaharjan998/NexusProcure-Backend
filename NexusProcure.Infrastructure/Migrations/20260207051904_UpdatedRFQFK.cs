using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedRFQFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_RequestForQuotations_RequestForQuotationId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_RequestForQuotationId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_RfqVendorId",
                table: "Quotations");

            migrationBuilder.DropColumn(
                name: "RequestForQuotationId",
                table: "Quotations");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_RfqId",
                table: "Quotations",
                column: "RfqId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_RfqVendorId",
                table: "Quotations",
                column: "RfqVendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_RequestForQuotations_RfqId",
                table: "Quotations",
                column: "RfqId",
                principalTable: "RequestForQuotations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Quotations_RequestForQuotations_RfqId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_RfqId",
                table: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_Quotations_RfqVendorId",
                table: "Quotations");

            migrationBuilder.AddColumn<Guid>(
                name: "RequestForQuotationId",
                table: "Quotations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_RequestForQuotationId",
                table: "Quotations",
                column: "RequestForQuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_RfqVendorId",
                table: "Quotations",
                column: "RfqVendorId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Quotations_RequestForQuotations_RequestForQuotationId",
                table: "Quotations",
                column: "RequestForQuotationId",
                principalTable: "RequestForQuotations",
                principalColumn: "Id");
        }
    }
}
