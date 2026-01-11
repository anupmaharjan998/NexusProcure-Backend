using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeApprovalLevelToRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalPolicies_ApprovalLevels_ApprovalLevelId",
                table: "ApprovalPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_ApprovalLevels_ApprovalLevelId",
                table: "Approvals");

            migrationBuilder.RenameColumn(
                name: "ApprovalLevelId",
                table: "Approvals",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Approvals_ApprovalLevelId",
                table: "Approvals",
                newName: "IX_Approvals_RoleId");

            migrationBuilder.RenameColumn(
                name: "ApprovalLevelId",
                table: "ApprovalPolicies",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalPolicies_ApprovalLevelId",
                table: "ApprovalPolicies",
                newName: "IX_ApprovalPolicies_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalPolicies_Roles_RoleId",
                table: "ApprovalPolicies",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_Roles_RoleId",
                table: "Approvals",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalPolicies_Roles_RoleId",
                table: "ApprovalPolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_Approvals_Roles_RoleId",
                table: "Approvals");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Approvals",
                newName: "ApprovalLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Approvals_RoleId",
                table: "Approvals",
                newName: "IX_Approvals_ApprovalLevelId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "ApprovalPolicies",
                newName: "ApprovalLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalPolicies_RoleId",
                table: "ApprovalPolicies",
                newName: "IX_ApprovalPolicies_ApprovalLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalPolicies_ApprovalLevels_ApprovalLevelId",
                table: "ApprovalPolicies",
                column: "ApprovalLevelId",
                principalTable: "ApprovalLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Approvals_ApprovalLevels_ApprovalLevelId",
                table: "Approvals",
                column: "ApprovalLevelId",
                principalTable: "ApprovalLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
