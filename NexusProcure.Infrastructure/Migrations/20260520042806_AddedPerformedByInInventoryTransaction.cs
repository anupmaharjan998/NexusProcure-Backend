using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPerformedByInInventoryTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_InventoryStocks_StockId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequestItems_InventoryItems_InventoryItemId",
                table: "InventoryRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequestItems_InventoryStocks_StockId",
                table: "InventoryRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_InventoryStocks_StockId",
                table: "InventoryTransactions");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryRequestItems_InventoryItemId",
                table: "InventoryRequestItems");

            migrationBuilder.DropColumn(
                name: "InventoryItemId",
                table: "InventoryRequestItems");

            migrationBuilder.AddColumn<int>(
                name: "ReorderLevel",
                table: "PurchaseOrderItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "PurchaseOrderItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "InventoryTransactions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "PerformedById",
                table: "InventoryTransactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "InventoryTransactions",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "StockId",
                table: "InventoryRequestItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "Condition",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "InventoryRequestIssuedItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryRequestItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRequestIssuedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRequestIssuedItems_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryRequestIssuedItems_InventoryRequestItems_Inventory~",
                        column: x => x.InventoryRequestItemId,
                        principalTable: "InventoryRequestItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_PerformedById",
                table: "InventoryTransactions",
                column: "PerformedById");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_CreatedById",
                table: "InventoryStocks",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_Name_CategoryId",
                table: "InventoryStocks",
                columns: new[] { "Name", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequestIssuedItems_InventoryItemId",
                table: "InventoryRequestIssuedItems",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequestIssuedItems_InventoryRequestItemId",
                table: "InventoryRequestIssuedItems",
                column: "InventoryRequestItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_InventoryStocks_StockId",
                table: "InventoryItems",
                column: "StockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequestItems_InventoryStocks_StockId",
                table: "InventoryRequestItems",
                column: "StockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStocks_Users_CreatedById",
                table: "InventoryStocks",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_InventoryStocks_StockId",
                table: "InventoryTransactions",
                column: "StockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Users_PerformedById",
                table: "InventoryTransactions",
                column: "PerformedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_InventoryStocks_StockId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequestItems_InventoryStocks_StockId",
                table: "InventoryRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStocks_Users_CreatedById",
                table: "InventoryStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_InventoryStocks_StockId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Users_PerformedById",
                table: "InventoryTransactions");

            migrationBuilder.DropTable(
                name: "InventoryRequestIssuedItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_PerformedById",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryStocks_CreatedById",
                table: "InventoryStocks");

            migrationBuilder.DropIndex(
                name: "IX_InventoryStocks_Name_CategoryId",
                table: "InventoryStocks");

            migrationBuilder.DropColumn(
                name: "ReorderLevel",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "PerformedById",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "InventoryTransactions");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "InventoryTransactions",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<Guid>(
                name: "StockId",
                table: "InventoryRequestItems",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryItemId",
                table: "InventoryRequestItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InventoryItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Condition",
                table: "InventoryItems",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "integer", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservedQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_InventoryItems_InventoryItemId1",
                        column: x => x.InventoryItemId1,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequestItems_InventoryItemId",
                table: "InventoryRequestItems",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_InventoryItemId1",
                table: "Stocks",
                column: "InventoryItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_InventoryStocks_StockId",
                table: "InventoryItems",
                column: "StockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequestItems_InventoryItems_InventoryItemId",
                table: "InventoryRequestItems",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequestItems_InventoryStocks_StockId",
                table: "InventoryRequestItems",
                column: "StockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_InventoryStocks_StockId",
                table: "InventoryTransactions",
                column: "StockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
