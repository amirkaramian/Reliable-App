using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class UpdateUserAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IPAddress",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Identity",
                table: "Users");
        }
    }
}
