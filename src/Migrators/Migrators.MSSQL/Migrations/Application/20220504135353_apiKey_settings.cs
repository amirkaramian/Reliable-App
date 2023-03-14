using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class apiKey_settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableThirdPartyAPIkeys",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "IntervalBeforeNextAPIkeyRequestInSeconds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberofRequestsPerIpApiKey",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableThirdPartyAPIkeys",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "IntervalBeforeNextAPIkeyRequestInSeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "NumberofRequestsPerIpApiKey",
                table: "Settings");
        }
    }
}
