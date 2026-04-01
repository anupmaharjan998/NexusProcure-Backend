using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InventoryAndUpdateVendorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalPolicies_Categories_CategoryId",
                table: "ApprovalPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAssignments_InventoryItems_InventoryItemId",
                table: "AssetAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Categories_CategoryId",
                table: "Requisitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Categories_CategoryId",
                table: "Vendors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "ItemName",
                table: "InventoryItems",
                newName: "SerialNumber");

            migrationBuilder.RenameColumn(
                name: "AssetTag",
                table: "InventoryItems",
                newName: "SKU");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId1",
                table: "Vendors",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AssignedDate",
                table: "InventoryItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToId",
                table: "InventoryItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "InventoryItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Condition",
                table: "InventoryItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "InventoryItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "InventoryItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryCategoryId",
                table: "InventoryItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "InventoryItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "InventoryItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Category",
                table: "Category",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DashboardStatsDto",
                columns: table => new
                {
                    total_users = table.Column<int>(type: "integer", nullable: false),
                    total_departments = table.Column<int>(type: "integer", nullable: false),
                    total_roles = table.Column<int>(type: "integer", nullable: false),
                    total_vendors = table.Column<int>(type: "integer", nullable: false),
                    total_requisitions = table.Column<int>(type: "integer", nullable: false),
                    pending_requisition_approvals = table.Column<int>(type: "integer", nullable: false),
                    total_quotations = table.Column<int>(type: "integer", nullable: false),
                    total_purchase_orders = table.Column<int>(type: "integer", nullable: false),
                    active_orders = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "DeliveryDto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    po_number = table.Column<string>(type: "text", nullable: false),
                    vendor_name = table.Column<string>(type: "text", nullable: false),
                    delivery_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_items = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceipts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceipts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsReturned = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryAssignments_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CategoryCode = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    RiskWeight = table.Column<int>(type: "integer", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryCategories_InventoryCategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "InventoryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetTag = table.Column<string>(type: "text", nullable: false),
                    ItemName = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryItem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecentPurchaseOrderDto",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    po_number = table.Column<string>(type: "text", nullable: false),
                    vendor_name = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric", nullable: false),
                    total_items = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    AvailableQuantity = table.Column<int>(type: "integer", nullable: false),
                    ReservedQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reference = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GoodsReceiptItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GoodsReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityReceived = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsReceiptItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsReceiptItem_GoodsReceipts_GoodsReceiptId",
                        column: x => x.GoodsReceiptId,
                        principalTable: "GoodsReceipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CategoryId1",
                table: "Vendors",
                column: "CategoryId1");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_AssignedToId",
                table: "InventoryItems",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_InventoryCategoryId",
                table: "InventoryItems",
                column: "InventoryCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_SKU",
                table: "InventoryItems",
                column: "SKU",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoodsReceiptItem_GoodsReceiptId",
                table: "GoodsReceiptItem",
                column: "GoodsReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAssignments_InventoryItemId",
                table: "InventoryAssignments",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCategories_CategoryCode",
                table: "InventoryCategories",
                column: "CategoryCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryCategories_ParentCategoryId",
                table: "InventoryCategories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_InventoryItemId",
                table: "Stocks",
                column: "InventoryItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalPolicies_Category_CategoryId",
                table: "ApprovalPolicies",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAssignments_InventoryItem_InventoryItemId",
                table: "AssetAssignments",
                column: "InventoryItemId",
                principalTable: "InventoryItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_InventoryCategories_InventoryCategoryId",
                table: "InventoryItems",
                column: "InventoryCategoryId",
                principalTable: "InventoryCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_Users_AssignedToId",
                table: "InventoryItems",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Category_CategoryId",
                table: "Requisitions",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalPolicies_Category_CategoryId",
                table: "ApprovalPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetAssignments_InventoryItem_InventoryItemId",
                table: "AssetAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_InventoryCategories_InventoryCategoryId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_Users_AssignedToId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Category_CategoryId",
                table: "Requisitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_Category_CategoryId1",
                table: "Vendors");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendors_InventoryCategories_CategoryId",
                table: "Vendors");

            migrationBuilder.DropTable(
                name: "DashboardStatsDto");

            migrationBuilder.DropTable(
                name: "DeliveryDto");

            migrationBuilder.DropTable(
                name: "GoodsReceiptItem");

            migrationBuilder.DropTable(
                name: "InventoryAssignments");

            migrationBuilder.DropTable(
                name: "InventoryCategories");

            migrationBuilder.DropTable(
                name: "InventoryItem");

            migrationBuilder.DropTable(
                name: "RecentPurchaseOrderDto");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "GoodsReceipts");

            migrationBuilder.DropIndex(
                name: "IX_Vendors_CategoryId1",
                table: "Vendors");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_AssignedToId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_InventoryCategoryId",
                table: "InventoryItems");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_SKU",
                table: "InventoryItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Category",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "AssignedDate",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Condition",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "InventoryCategoryId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "InventoryItems");

            migrationBuilder.RenameTable(
                name: "Category",
                newName: "Categories");

            migrationBuilder.RenameColumn(
                name: "SerialNumber",
                table: "InventoryItems",
                newName: "ItemName");

            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "InventoryItems",
                newName: "AssetTag");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "InventoryItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalPolicies_Categories_CategoryId",
                table: "ApprovalPolicies",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetAssignments_InventoryItems_InventoryItemId",
                table: "AssetAssignments",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Categories_CategoryId",
                table: "Requisitions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendors_Categories_CategoryId",
                table: "Vendors",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
