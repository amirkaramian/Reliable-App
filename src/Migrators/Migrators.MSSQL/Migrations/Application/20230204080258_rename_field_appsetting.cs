using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class rename_field_appsetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvaillableForOrders",
                table: "UserAppSettings",
                newName: "AvaillableForOrders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvaillableForOrders",
                table: "UserAppSettings",
                newName: "AvaillableForOrders");
        }
    }
}
