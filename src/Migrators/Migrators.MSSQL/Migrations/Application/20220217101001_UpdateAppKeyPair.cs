using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class UpdateAppKeyPair : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Permissions",
            table: "APIKeyPairs",
            newName: "Roles");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Roles",
            table: "APIKeyPairs",
            newName: "Permissions");
    }
}
