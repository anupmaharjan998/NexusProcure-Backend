using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryRequestWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_InventoryCategories_InventoryCategoryId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Users_CreatedById",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequestItems_InventoryStocks_StockId",
                table: "InventoryRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequests_Departments_DepartmentId",
                table: "InventoryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequests_Users_ApprovedByManagerId",
                table: "InventoryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequests_Users_ProcessedByInventoryManagerId",
                table: "InventoryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequests_Users_RequestedById",
                table: "InventoryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStocks_InventoryCategories_CategoryId",
                table: "InventoryStocks");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "InventoryTransactions",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryCategoryId",
                table: "InventoryStocks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "InventoryStocks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InventoryRequests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "InventoryRequests",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "InventoryRequests",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Priority",
                table: "InventoryRequests",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "InventoryRequests",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "InventoryItems",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "Condition",
                table: "InventoryItems",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryCategoryId1",
                table: "InventoryItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "InventoryItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "InventoryAssignmentHistories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_TransactionDate",
                table: "InventoryTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_InventoryCategoryId",
                table: "InventoryStocks",
                column: "InventoryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_CreatedAt",
                table: "InventoryRequests",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_Status",
                table: "InventoryRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequestIssuedItems_InventoryRequestItemId_Inventor~",
                table: "InventoryRequestIssuedItems",
                columns: new[] { "InventoryRequestItemId", "InventoryItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_InventoryCategoryId1",
                table: "InventoryItems",
                column: "InventoryCategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_InventoryCategories_InventoryCategoryId",
                table: "InventoryItems",
                column: "InventoryCategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_InventoryCategories_InventoryCategoryId1",
                table: "InventoryItems",
                column: "InventoryCategoryId1",
                principalTable: "InventoryCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Users_CreatedById",
                table: "InventoryItems",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequestItems_InventoryStocks_StockId",
                table: "InventoryRequestItems",
                column: "StockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequests_Departments_DepartmentId",
                table: "InventoryRequests",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequests_Users_ApprovedByManagerId",
                table: "InventoryRequests",
                column: "ApprovedByManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequests_Users_ProcessedByInventoryManagerId",
                table: "InventoryRequests",
                column: "ProcessedByInventoryManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequests_Users_RequestedById",
                table: "InventoryRequests",
                column: "RequestedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStocks_InventoryCategories_CategoryId",
                table: "InventoryStocks",
                column: "CategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStocks_InventoryCategories_InventoryCategoryId",
                table: "InventoryStocks",
                column: "InventoryCategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_InventoryCategories_InventoryCategoryId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_InventoryCategories_InventoryCategoryId1",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Users_CreatedById",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequestItems_InventoryStocks_StockId",
                table: "InventoryRequestItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequests_Departments_DepartmentId",
                table: "InventoryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequests_Users_ApprovedByManagerId",
                table: "InventoryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequests_Users_ProcessedByInventoryManagerId",
                table: "InventoryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryRequests_Users_RequestedById",
                table: "InventoryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStocks_InventoryCategories_CategoryId",
                table: "InventoryStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryStocks_InventoryCategories_InventoryCategoryId",
                table: "InventoryStocks");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_TransactionDate",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryStocks_InventoryCategoryId",
                table: "InventoryStocks");

            migrationBuilder.DropIndex(
                name: "IX_InventoryRequests_CreatedAt",
                table: "InventoryRequests");

            migrationBuilder.DropIndex(
                name: "IX_InventoryRequests_Status",
                table: "InventoryRequests");

            migrationBuilder.DropIndex(
                name: "IX_InventoryRequestIssuedItems_InventoryRequestItemId_Inventor~",
                table: "InventoryRequestIssuedItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_InventoryCategoryId1",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "InventoryCategoryId",
                table: "InventoryStocks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "InventoryStocks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "InventoryRequests");

            migrationBuilder.DropColumn(
                name: "InventoryCategoryId1",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "InventoryAssignmentHistories");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "InventoryTransactions",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InventoryRequests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "InventoryRequests",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Purpose",
                table: "InventoryRequests",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "InventoryRequests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<int>(
                name: "Condition",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_InventoryCategories_InventoryCategoryId",
                table: "InventoryItems",
                column: "InventoryCategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Users_CreatedById",
                table: "InventoryItems",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequestItems_InventoryStocks_StockId",
                table: "InventoryRequestItems",
                column: "StockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequests_Departments_DepartmentId",
                table: "InventoryRequests",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequests_Users_ApprovedByManagerId",
                table: "InventoryRequests",
                column: "ApprovedByManagerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequests_Users_ProcessedByInventoryManagerId",
                table: "InventoryRequests",
                column: "ProcessedByInventoryManagerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryRequests_Users_RequestedById",
                table: "InventoryRequests",
                column: "RequestedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryStocks_InventoryCategories_CategoryId",
                table: "InventoryStocks",
                column: "CategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
