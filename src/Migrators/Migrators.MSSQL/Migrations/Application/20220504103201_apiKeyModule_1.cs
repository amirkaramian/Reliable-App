using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class apiKeyModule_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApiKeyModule_APIKeyPairs_APIKeyPairId",
                table: "UserApiKeyModule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserApiKeyModule",
                table: "UserApiKeyModule");

            migrationBuilder.RenameTable(
                name: "UserApiKeyModule",
                newName: "UserApiKeyModules");

            migrationBuilder.RenameIndex(
                name: "IX_UserApiKeyModule_APIKeyPairId",
                table: "UserApiKeyModules",
                newName: "IX_UserApiKeyModules_APIKeyPairId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserApiKeyModules",
                table: "UserApiKeyModules",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApiKeyModules_APIKeyPairs_APIKeyPairId",
                table: "UserApiKeyModules",
                column: "APIKeyPairId",
                principalTable: "APIKeyPairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserApiKeyModules_APIKeyPairs_APIKeyPairId",
                table: "UserApiKeyModules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserApiKeyModules",
                table: "UserApiKeyModules");

            migrationBuilder.RenameTable(
                name: "UserApiKeyModules",
                newName: "UserApiKeyModule");

            migrationBuilder.RenameIndex(
                name: "IX_UserApiKeyModules_APIKeyPairId",
                table: "UserApiKeyModule",
                newName: "IX_UserApiKeyModule_APIKeyPairId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserApiKeyModule",
                table: "UserApiKeyModule",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserApiKeyModule_APIKeyPairs_APIKeyPairId",
                table: "UserApiKeyModule",
                column: "APIKeyPairId",
                principalTable: "APIKeyPairs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
