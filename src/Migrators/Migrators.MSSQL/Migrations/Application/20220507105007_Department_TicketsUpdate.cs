using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Department_TicketsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentIds",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssignedTo",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketPriority",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketRelatedTo",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TicketRelatedToId",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketItems",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Departments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Departments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ApplicationUserId",
                table: "Departments",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Users_ApplicationUserId",
                table: "Departments",
                column: "ApplicationUserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Users_ApplicationUserId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ApplicationUserId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DepartmentIds",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketPriority",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketRelatedTo",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketRelatedToId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketItems",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Departments");
        }
    }
}
