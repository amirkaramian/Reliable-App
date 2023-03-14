using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class BillStatusColRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bills");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Bills",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
