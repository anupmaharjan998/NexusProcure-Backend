using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedApprovalPolicyAndDelegation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Users_ApprovedById",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "MaxAmount",
                table: "ApprovalLevels");

            migrationBuilder.DropColumn(
                name: "MinAmount",
                table: "ApprovalLevels");

            migrationBuilder.RenameColumn(
                name: "Decision",
                table: "Approvals",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "ApprovedDate",
                table: "Approvals",
                newName: "AssignedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Requisitions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsUrgent",
                table: "Requisitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RiskLevel",
                table: "Requisitions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "RiskScore",
                table: "Requisitions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RiskWeight",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "Approvals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApprovedById",
                table: "Approvals",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActionedAt",
                table: "Approvals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovalLevelId",
                table: "Approvals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToUserId",
                table: "Approvals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "Escalated",
                table: "Approvals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EscalatedAt",
                table: "Approvals",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApprovalDelegations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ToUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalDelegations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalPolicies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RiskLevel = table.Column<int>(type: "integer", nullable: false),
                    ApprovalLevelId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceOrder = table.Column<int>(type: "integer", nullable: false),
                    EscalationHours = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalPolicies_ApprovalLevels_ApprovalLevelId",
                        column: x => x.ApprovalLevelId,
                        principalTable: "ApprovalLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalPolicies_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Description", "Group", "Key" },
                values: new object[,]
                {
                    { new Guid("11000000-0000-0000-0000-000000000001"), "View permissions", "Permissions", "VIEW_PERMISSIONS" },
                    { new Guid("90000000-0000-0000-0000-000000000001"), "View category", "Category", "VIEW_CATEGORIES" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("11000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") },
                    { new Guid("90000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_CategoryId",
                table: "Requisitions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_ApprovalLevelId",
                table: "Approvals",
                column: "ApprovalLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Approvals_AssignedToUserId",
                table: "Approvals",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalPolicies_ApprovalLevelId",
                table: "ApprovalPolicies",
                column: "ApprovalLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalPolicies_CategoryId",
                table: "ApprovalPolicies",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_ApprovalLevels_ApprovalLevelId",
                table: "Approvals",
                column: "ApprovalLevelId",
                principalTable: "ApprovalLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Users_ApprovedById",
                table: "Approvals",
                column: "ApprovedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Users_AssignedToUserId",
                table: "Approvals",
                column: "AssignedToUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Categories_CategoryId",
                table: "Requisitions",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_ApprovalLevels_ApprovalLevelId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Users_ApprovedById",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Users_AssignedToUserId",
                table: "Approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Categories_CategoryId",
                table: "Requisitions");

            migrationBuilder.DropTable(
                name: "ApprovalDelegations");

            migrationBuilder.DropTable(
                name: "ApprovalPolicies");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_CategoryId",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_ApprovalLevelId",
                table: "Approvals");

            migrationBuilder.DropIndex(
                name: "IX_Approvals_AssignedToUserId",
                table: "Approvals");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("11000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("90000000-0000-0000-0000-000000000001"), new Guid("c76abcb8-63b5-4e14-8428-3a9a9b7ad001") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("11000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("90000000-0000-0000-0000-000000000001"));

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "IsUrgent",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "RiskLevel",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "RiskScore",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "RiskWeight",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ActionedAt",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "ApprovalLevelId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "Escalated",
                table: "Approvals");

            migrationBuilder.DropColumn(
                name: "EscalatedAt",
                table: "Approvals");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Approvals",
                newName: "Decision");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "Approvals",
                newName: "ApprovedDate");

            migrationBuilder.AlterColumn<string>(
                name: "Comments",
                table: "Approvals",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApprovedById",
                table: "Approvals",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxAmount",
                table: "ApprovalLevels",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "MinAmount",
                table: "ApprovalLevels",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Users_ApprovedById",
                table: "Approvals",
                column: "ApprovedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
