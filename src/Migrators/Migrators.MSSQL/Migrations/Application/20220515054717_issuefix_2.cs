using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class issuefix_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAppSettingss",
                table: "UserAppSettingss");

            migrationBuilder.RenameTable(
                name: "UserAppSettingss",
                newName: "UserAppSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAppSettings",
                table: "UserAppSettings",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAppSettings",
                table: "UserAppSettings");

            migrationBuilder.RenameTable(
                name: "UserAppSettings",
                newName: "UserAppSettingss");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAppSettingss",
                table: "UserAppSettingss",
                column: "Id");
        }
    }
}
