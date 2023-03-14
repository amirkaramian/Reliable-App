using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class update_PaymentGateway : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "PaymentGateways",
                newName: "Name");

            migrationBuilder.AddColumn<bool>(
                name: "EnableLoginIntervalInSeconds_PortalSettings",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LoginIntervalInSeconds_PortalSettings",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "PaymentGateways",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "PaymentGateways",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableLoginIntervalInSeconds_PortalSettings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "LoginIntervalInSeconds_PortalSettings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "PaymentGateways");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PaymentGateways");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "PaymentGateways",
                newName: "Description");
        }
    }
}
