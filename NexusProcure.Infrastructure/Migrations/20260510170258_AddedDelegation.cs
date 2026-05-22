using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDelegation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_PerformedById",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAssignmentHistories_Users_AssignedById",
                table: "InventoryAssignmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAssignmentHistories_Users_UnassignedById",
                table: "InventoryAssignmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_InventoryItems_InventoryItemId",
                table: "Stocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_DelegateUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DelegateUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_InventoryItemId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_InventoryAssignmentHistories_UnassignedById",
                table: "InventoryAssignmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_PerformedById",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "DelegateUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UnassignedById",
                table: "InventoryAssignmentHistories");

            migrationBuilder.RenameColumn(
                name: "AssignedById",
                table: "InventoryAssignmentHistories",
                newName: "PerformedById");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryAssignmentHistories_AssignedById",
                table: "InventoryAssignmentHistories",
                newName: "IX_InventoryAssignmentHistories_PerformedById");

            migrationBuilder.RenameColumn(
                name: "PerformedById",
                table: "AuditLogs",
                newName: "EntityId");

            migrationBuilder.RenameColumn(
                name: "Details",
                table: "AuditLogs",
                newName: "EntityName");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryItemId1",
                table: "Stocks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignedDate",
                table: "InventoryItems",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<Guid>(
                name: "StockId",
                table: "InventoryItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsAssetTracked",
                table: "InventoryCategories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "InventoryAssignmentHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AuditLogs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NewValues",
                table: "AuditLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldValues",
                table: "AuditLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PerformedBy",
                table: "AuditLogs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InventoryRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ApprovedByManagerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProcessedByInventoryManagerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryRequests_Users_ApprovedByManagerId",
                        column: x => x.ApprovedByManagerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryRequests_Users_ProcessedByInventoryManagerId",
                        column: x => x.ProcessedByInventoryManagerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryRequests_Users_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InventoryStocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    SKU = table.Column<string>(type: "text", nullable: false),
                    QuantityAvailable = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Unit = table.Column<string>(type: "text", nullable: false),
                    ReorderLevel = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryStocks_InventoryCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "InventoryCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedById = table.Column<Guid>(type: "uuid", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Purpose = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsAutoGenerated = table.Column<bool>(type: "boolean", nullable: false),
                    SourceInventoryRequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementRequests_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcurementRequests_Users_RequestedById",
                        column: x => x.RequestedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDelegations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DelegateUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDelegations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDelegations_Users_DelegateUserId",
                        column: x => x.DelegateUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserDelegations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InventoryRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InventoryRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    StockId = table.Column<Guid>(type: "uuid", nullable: true),
                    InventoryItemId = table.Column<Guid>(type: "uuid", nullable: true),
                    QuantityRequested = table.Column<int>(type: "integer", nullable: false),
                    QuantityIssued = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRequestItems_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InventoryRequestItems_InventoryRequests_InventoryRequestId",
                        column: x => x.InventoryRequestId,
                        principalTable: "InventoryRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryRequestItems_InventoryStocks_StockId",
                        column: x => x.StockId,
                        principalTable: "InventoryStocks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "InventoryTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityChange = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryTransactions_InventoryStocks_StockId",
                        column: x => x.StockId,
                        principalTable: "InventoryStocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcurementRequestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcurementRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcurementRequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcurementRequestItems_ProcurementRequests_ProcurementRequ~",
                        column: x => x.ProcurementRequestId,
                        principalTable: "ProcurementRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_InventoryItemId1",
                table: "Stocks",
                column: "InventoryItemId1");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryItems_StockId",
                table: "InventoryItems",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequestItems_InventoryItemId",
                table: "InventoryRequestItems",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequestItems_InventoryRequestId",
                table: "InventoryRequestItems",
                column: "InventoryRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequestItems_StockId",
                table: "InventoryRequestItems",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_ApprovedByManagerId",
                table: "InventoryRequests",
                column: "ApprovedByManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_DepartmentId",
                table: "InventoryRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_ProcessedByInventoryManagerId",
                table: "InventoryRequests",
                column: "ProcessedByInventoryManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRequests_RequestedById",
                table: "InventoryRequests",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryStocks_CategoryId",
                table: "InventoryStocks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_StockId",
                table: "InventoryTransactions",
                column: "StockId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementRequestItems_ProcurementRequestId",
                table: "ProcurementRequestItems",
                column: "ProcurementRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementRequests_DepartmentId",
                table: "ProcurementRequests",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcurementRequests_RequestedById",
                table: "ProcurementRequests",
                column: "RequestedById");

            migrationBuilder.CreateIndex(
                name: "IX_UserDelegations_DelegateUserId",
                table: "UserDelegations",
                column: "DelegateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDelegations_UserId",
                table: "UserDelegations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAssignmentHistories_Users_PerformedById",
                table: "InventoryAssignmentHistories",
                column: "PerformedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryItems_InventoryStocks_StockId",
                table: "InventoryItems",
                column: "StockId",
                principalTable: "InventoryStocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_InventoryItems_InventoryItemId1",
                table: "Stocks",
                column: "InventoryItemId1",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryAssignmentHistories_Users_PerformedById",
                table: "InventoryAssignmentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryItems_InventoryStocks_StockId",
                table: "InventoryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_InventoryItems_InventoryItemId1",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "InventoryRequestItems");

            migrationBuilder.DropTable(
                name: "InventoryTransactions");

            migrationBuilder.DropTable(
                name: "ProcurementRequestItems");

            migrationBuilder.DropTable(
                name: "UserDelegations");

            migrationBuilder.DropTable(
                name: "InventoryRequests");

            migrationBuilder.DropTable(
                name: "InventoryStocks");

            migrationBuilder.DropTable(
                name: "ProcurementRequests");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_InventoryItemId1",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_InventoryItems_StockId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "InventoryItemId1",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "StockId",
                table: "InventoryItems");

            migrationBuilder.DropColumn(
                name: "IsAssetTracked",
                table: "InventoryCategories");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "InventoryAssignmentHistories");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "NewValues",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "OldValues",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "PerformedBy",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "PerformedById",
                table: "InventoryAssignmentHistories",
                newName: "AssignedById");

            migrationBuilder.RenameIndex(
                name: "IX_InventoryAssignmentHistories_PerformedById",
                table: "InventoryAssignmentHistories",
                newName: "IX_InventoryAssignmentHistories_AssignedById");

            migrationBuilder.RenameColumn(
                name: "EntityName",
                table: "AuditLogs",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "AuditLogs",
                newName: "PerformedById");

            migrationBuilder.AddColumn<Guid>(
                name: "DelegateUserId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AssignedDate",
                table: "InventoryItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnassignedById",
                table: "InventoryAssignmentHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a87f3d2b-0f0d-4b4e-9d2a-4e09d68f4103"),
                column: "DelegateUserId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a87f3d2b-0f0d-4b4e-9d2a-4e09d68f4104"),
                column: "DelegateUserId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DelegateUserId",
                table: "Users",
                column: "DelegateUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_InventoryItemId",
                table: "Stocks",
                column: "InventoryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryAssignmentHistories_UnassignedById",
                table: "InventoryAssignmentHistories",
                column: "UnassignedById");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_PerformedById",
                table: "AuditLogs",
                column: "PerformedById");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_PerformedById",
                table: "AuditLogs",
                column: "PerformedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAssignmentHistories_Users_AssignedById",
                table: "InventoryAssignmentHistories",
                column: "AssignedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryAssignmentHistories_Users_UnassignedById",
                table: "InventoryAssignmentHistories",
                column: "UnassignedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_InventoryItems_InventoryItemId",
                table: "Stocks",
                column: "InventoryItemId",
                principalTable: "InventoryItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_DelegateUserId",
                table: "Users",
                column: "DelegateUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
