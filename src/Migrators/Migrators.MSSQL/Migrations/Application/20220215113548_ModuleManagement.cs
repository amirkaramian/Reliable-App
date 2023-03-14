using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class ModuleManagement : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "LogRotation",
            table: "Settings",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<int>(
            name: "LogRotationDays",
            table: "Settings",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "PermissionDetail",
            table: "Modules",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
           name: "IsActive",
           table: "Modules",
           type: "bit",
           nullable: false,
           defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "LogRotation",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "LogRotationDays",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "PermissionDetail",
            table: "Modules");

        migrationBuilder.DropColumn(
           name: "IsActive",
           table: "Modules");
    }
}
