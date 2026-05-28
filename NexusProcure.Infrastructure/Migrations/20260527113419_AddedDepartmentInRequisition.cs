using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusProcure.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDepartmentInRequisition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Departments_DepartmentId",
                table: "Requisitions");

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId1",
                table: "Requisitions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requisitions_DepartmentId1",
                table: "Requisitions",
                column: "DepartmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Departments_DepartmentId",
                table: "Requisitions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Departments_DepartmentId1",
                table: "Requisitions",
                column: "DepartmentId1",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Departments_DepartmentId",
                table: "Requisitions");

            migrationBuilder.DropForeignKey(
                name: "FK_Requisitions_Departments_DepartmentId1",
                table: "Requisitions");

            migrationBuilder.DropIndex(
                name: "IX_Requisitions_DepartmentId1",
                table: "Requisitions");

            migrationBuilder.DropColumn(
                name: "DepartmentId1",
                table: "Requisitions");

            migrationBuilder.AddForeignKey(
                name: "FK_Requisitions_Departments_DepartmentId",
                table: "Requisitions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
