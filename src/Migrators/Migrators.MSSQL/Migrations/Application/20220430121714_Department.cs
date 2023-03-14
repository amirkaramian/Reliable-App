using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Department : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AdminGroups_AdminGroupId1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AdminGroupId1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdminGroupId1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "UserLoginHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "UserLoginHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "UserLoginHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "UserLoginHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "UserLoginHistories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifiedBy",
                table: "UserLoginHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "UserLoginHistories",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "UserLoginHistories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserLoginHistories");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "UserLoginHistories");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "UserLoginHistories");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "UserLoginHistories");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "UserLoginHistories");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "UserLoginHistories");

            migrationBuilder.AddColumn<Guid>(
                name: "AdminGroupId1",
                schema: "Identity",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AdminGroupId1",
                schema: "Identity",
                table: "Users",
                column: "AdminGroupId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AdminGroups_AdminGroupId1",
                schema: "Identity",
                table: "Users",
                column: "AdminGroupId1",
                principalTable: "AdminGroups",
                principalColumn: "Id");
        }
    }
}
