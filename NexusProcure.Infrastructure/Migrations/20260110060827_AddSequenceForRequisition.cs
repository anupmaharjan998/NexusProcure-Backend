using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSequenceForRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "requisition_number_seq");

            migrationBuilder.AddColumn<string>(
                name: "RequisitionNumber",
                table: "Requisitions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Approvals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SequenceOrder",
                table: "Approvals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_RequisitionNumber",
                table: "Requisitions",
                column: "RequisitionNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requisitions_RequisitionNumber",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "RequisitionNumber",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "SequenceOrder",
                table: "Approvals");

            migrationBuilder.DropSequence(
                name: "requisition_number_seq");
        }
    }
}
