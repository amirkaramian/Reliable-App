using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Update_SettingsWithBilling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "TermsOfServiceAgreement",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "GoogleAuthenticator",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxNumberOfRefunds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "MicrosoftAuthenticator",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MinOrderAmount",
                table: "Settings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Module1Settings",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Module2Settings",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleAuthenticator",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "MaxNumberOfRefunds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "MicrosoftAuthenticator",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "MinOrderAmount",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Module1Settings",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "Module2Settings",
                table: "Settings");

            migrationBuilder.AlterColumn<string>(
                name: "TermsOfServiceAgreement",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
