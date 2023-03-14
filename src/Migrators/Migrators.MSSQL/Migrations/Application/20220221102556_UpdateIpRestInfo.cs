using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class UpdateIpRestInfo : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "requestsIntervalPerIPAfterLimitInSecondsOverwrite",
            schema: "Identity",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "requestsPerIPOverwrite",
            schema: "Identity",
            table: "Users",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "requestsIntervalPerIPAfterLimitInSecondsOverwrite",
            schema: "Identity",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "requestsPerIPOverwrite",
            schema: "Identity",
            table: "Users");
    }
}
