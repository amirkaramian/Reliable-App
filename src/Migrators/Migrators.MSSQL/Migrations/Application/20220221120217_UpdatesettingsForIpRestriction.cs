using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class UpdatesettingsForIpRestriction : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "requestsPerIPOverwrite",
            schema: "Identity",
            table: "Users",
            newName: "RequestsPerIPOverwrite");

        migrationBuilder.RenameColumn(
            name: "requestsIntervalPerIPAfterLimitInSecondsOverwrite",
            schema: "Identity",
            table: "Users",
            newName: "RequestsIntervalPerIPAfterLimitInSecondsOverwrite");

        migrationBuilder.RenameColumn(
            name: "requestsPerIPAdmin",
            table: "Settings",
            newName: "RequestsPerIPAdmin");

        migrationBuilder.RenameColumn(
            name: "requestsIntervalPerIPAfterLimitClientInSeconds",
            table: "Settings",
            newName: "RequestsIntervalPerIPAfterLimitClientInSeconds");

        migrationBuilder.RenameColumn(
            name: "requestsIntervalPerIPAfterLimitAdminInSeconds",
            table: "Settings",
            newName: "RequestsIntervalPerIPAfterLimitAdminInSeconds");

        migrationBuilder.RenameColumn(
            name: "loginRequestsPerIPClient",
            table: "Settings",
            newName: "LoginRequestsPerIPClient");

        migrationBuilder.RenameColumn(
            name: "loginRequestsPerIPAdmin",
            table: "Settings",
            newName: "LoginRequestsPerIPAdmin");

        migrationBuilder.RenameColumn(
            name: "enableAPIAccessClient",
            table: "Settings",
            newName: "EnableAPIAccessClient");

        migrationBuilder.RenameColumn(
            name: "enableAPIAccessAdmin",
            table: "Settings",
            newName: "EnableAPIAccessAdmin");

        migrationBuilder.AddColumn<int>(
            name: "RequestsPerIPClient",
            table: "Settings",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "RequestsPerIPClient",
            table: "Settings");

        migrationBuilder.RenameColumn(
            name: "RequestsPerIPOverwrite",
            schema: "Identity",
            table: "Users",
            newName: "requestsPerIPOverwrite");

        migrationBuilder.RenameColumn(
            name: "RequestsIntervalPerIPAfterLimitInSecondsOverwrite",
            schema: "Identity",
            table: "Users",
            newName: "requestsIntervalPerIPAfterLimitInSecondsOverwrite");

        migrationBuilder.RenameColumn(
            name: "RequestsPerIPAdmin",
            table: "Settings",
            newName: "requestsPerIPAdmin");

        migrationBuilder.RenameColumn(
            name: "RequestsIntervalPerIPAfterLimitClientInSeconds",
            table: "Settings",
            newName: "requestsIntervalPerIPAfterLimitClientInSeconds");

        migrationBuilder.RenameColumn(
            name: "RequestsIntervalPerIPAfterLimitAdminInSeconds",
            table: "Settings",
            newName: "requestsIntervalPerIPAfterLimitAdminInSeconds");

        migrationBuilder.RenameColumn(
            name: "LoginRequestsPerIPClient",
            table: "Settings",
            newName: "loginRequestsPerIPClient");

        migrationBuilder.RenameColumn(
            name: "LoginRequestsPerIPAdmin",
            table: "Settings",
            newName: "loginRequestsPerIPAdmin");

        migrationBuilder.RenameColumn(
            name: "EnableAPIAccessClient",
            table: "Settings",
            newName: "enableAPIAccessClient");

        migrationBuilder.RenameColumn(
            name: "EnableAPIAccessAdmin",
            table: "Settings",
            newName: "enableAPIAccessAdmin");
    }
}