using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class recordstodisplayinprofile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordsToDisplay",
                table: "UserAppSettings");

            migrationBuilder.AddColumn<int>(
                name: "RecordsToDisplay",
                schema: "Identity",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecordsToDisplay",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "RecordsToDisplay",
                table: "UserAppSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
