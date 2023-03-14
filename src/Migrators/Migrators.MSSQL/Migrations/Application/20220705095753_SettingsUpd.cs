using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class SettingsUpd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ForceMFA",
                table: "Settings",
                newName: "ForceClientMFA");

            migrationBuilder.AddColumn<bool>(
                name: "ForceAdminMFA",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoInvoiceGeneration",
                table: "BillingSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "AutoInvoicePriorToDueDateInDays",
                table: "BillingSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableProductlevelInvoiceGen",
                table: "BillingSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ProductLevelInvoiceGenPriorToDueDateInDays",
                table: "BillingSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsSuperAdmin",
                table: "AdminGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForceAdminMFA",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "AutoInvoiceGeneration",
                table: "BillingSettings");

            migrationBuilder.DropColumn(
                name: "AutoInvoicePriorToDueDateInDays",
                table: "BillingSettings");

            migrationBuilder.DropColumn(
                name: "EnableProductlevelInvoiceGen",
                table: "BillingSettings");

            migrationBuilder.DropColumn(
                name: "ProductLevelInvoiceGenPriorToDueDateInDays",
                table: "BillingSettings");

            migrationBuilder.DropColumn(
                name: "IsSuperAdmin",
                table: "AdminGroups");

            migrationBuilder.RenameColumn(
                name: "ForceClientMFA",
                table: "Settings",
                newName: "ForceMFA");
        }
    }
}
