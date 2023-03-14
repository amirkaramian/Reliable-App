using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class issuefix_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId1",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_DepartmentId1",
                table: "Tickets",
                column: "DepartmentId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Departments_DepartmentId1",
                table: "Tickets",
                column: "DepartmentId1",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Departments_DepartmentId1",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_DepartmentId1",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "DepartmentId1",
                table: "Tickets");
        }
    }
}
