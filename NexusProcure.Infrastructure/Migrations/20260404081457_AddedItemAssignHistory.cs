using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedItemAssignHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Users_AssignedToId",
                table: "InventoryItems");

            migrationBuilder.CreateTable(
                name: "InventoryAssignmentHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedToId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedById = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UnassignedById = table.Column<Guid>(type: "uuid", nullable: true),
                    UnassignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAssignmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAssignmentHistories_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryAssignmentHistories_Users_AssignedById",
                        column: x => x.AssignedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAssignmentHistories_Users_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryAssignmentHistories_Users_UnassignedById",
                        column: x => x.UnassignedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAssignmentHistories_AssignedById",
                table: "InventoryAssignmentHistories",
                column: "AssignedById");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAssignmentHistories_AssignedToId",
                table: "InventoryAssignmentHistories",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAssignmentHistories_InventoryItemId",
                table: "InventoryAssignmentHistories",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAssignmentHistories_UnassignedById",
                table: "InventoryAssignmentHistories",
                column: "UnassignedById");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Users_AssignedToId",
                table: "InventoryItems",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Users_AssignedToId",
                table: "InventoryItems");

            migrationBuilder.DropTable(
                name: "InventoryAssignmentHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Users_AssignedToId",
                table: "InventoryItems",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
