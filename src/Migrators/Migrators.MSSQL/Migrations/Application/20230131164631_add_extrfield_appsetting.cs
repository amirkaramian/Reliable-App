using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class add_extrfield_appsetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoAssignOrders",
                table: "UserAppSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AvaillableForOrders",
                table: "UserAppSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanTakeOrders",
                table: "UserAppSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoAssignOrders",
                table: "UserAppSettings");

            migrationBuilder.DropColumn(
                name: "AvaillableForOrders",
                table: "UserAppSettings");

            migrationBuilder.DropColumn(
                name: "CanTakeOrders",
                table: "UserAppSettings");
        }
    }
}
