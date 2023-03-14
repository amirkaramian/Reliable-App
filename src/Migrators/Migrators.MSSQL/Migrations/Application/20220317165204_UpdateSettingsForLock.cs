using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class UpdateSettingsForLock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "WebHooksHistory");

            migrationBuilder.AddColumn<int>(
                name: "DefaultInactivityMinutesLockAdmin",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DefaultInactivityMinutesLockClient",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultInactivityMinutesLockAdmin",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "DefaultInactivityMinutesLockClient",
                table: "Settings");

            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "WebHooksHistory",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
