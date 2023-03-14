using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class UserSettingIpAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRestrictedIps_Users_ApplicationUserId",
                table: "UserRestrictedIps");

            migrationBuilder.DropIndex(
                name: "IX_UserRestrictedIps_ApplicationUserId",
                table: "UserRestrictedIps");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "UserRestrictedIps");

            migrationBuilder.DropColumn(
                name: "RestrictAccessToIPAddress",
                table: "UserAppSettings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "UserRestrictedIps",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RestrictAccessToIPAddress",
                table: "UserAppSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRestrictedIps_ApplicationUserId",
                table: "UserRestrictedIps",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserRestrictedIps_Users_ApplicationUserId",
                table: "UserRestrictedIps",
                column: "ApplicationUserId",
                principalSchema: "Identity",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
