using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class MFAOTPadd : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "MFARequest",
            schema: "Identity",
            table: "Users",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "OTPCode",
            schema: "Identity",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<bool>(
            name: "enableAPIAccessAdmin",
            table: "Settings",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<bool>(
            name: "enableAPIAccessClient",
            table: "Settings",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<int>(
            name: "loginRequestsPerIPAdmin",
            table: "Settings",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "loginRequestsPerIPClient",
            table: "Settings",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "requestsIntervalPerIPAfterLimitAdminInSeconds",
            table: "Settings",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "requestsIntervalPerIPAfterLimitClientInSeconds",
            table: "Settings",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "requestsPerIPAdmin",
            table: "Settings",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MFARequest",
            schema: "Identity",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "OTPCode",
            schema: "Identity",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "enableAPIAccessAdmin",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "enableAPIAccessClient",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "loginRequestsPerIPAdmin",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "loginRequestsPerIPClient",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "requestsIntervalPerIPAfterLimitAdminInSeconds",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "requestsIntervalPerIPAfterLimitClientInSeconds",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "requestsPerIPAdmin",
            table: "Settings");
    }
}