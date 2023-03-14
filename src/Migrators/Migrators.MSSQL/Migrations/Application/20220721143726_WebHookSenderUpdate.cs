using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class WebHookSenderUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "WebHooks");

            migrationBuilder.RenameColumn(
                name: "Secret",
                table: "WebHooks",
                newName: "ModuleId");

            migrationBuilder.RenameColumn(
                name: "HookEvents",
                table: "WebHooks",
                newName: "Action");

            migrationBuilder.AddColumn<Guid>(
                name: "ModuleId1",
                table: "WebHooks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebHooks_ModuleId1",
                table: "WebHooks",
                column: "ModuleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_WebHooks_Modules_ModuleId1",
                table: "WebHooks",
                column: "ModuleId1",
                principalTable: "Modules",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WebHooks_Modules_ModuleId1",
                table: "WebHooks");

            migrationBuilder.DropIndex(
                name: "IX_WebHooks_ModuleId1",
                table: "WebHooks");

            migrationBuilder.DropColumn(
                name: "ModuleId1",
                table: "WebHooks");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "WebHooks",
                newName: "Secret");

            migrationBuilder.RenameColumn(
                name: "Action",
                table: "WebHooks",
                newName: "HookEvents");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "WebHooks",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
