using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class LoginHistory_AdminGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IPAddress",
                schema: "Identity",
                table: "Users",
                newName: "RestrictAccessIPAddress");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                schema: "Identity",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminGroupId",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminGroupId1",
                schema: "Identity",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoggedIn",
                schema: "Identity",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AdminGroupModules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissionDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    AdminGroupId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminAsClient = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminGroupModules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdminGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminAsClient = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLoginHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoginTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeviceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLoginHistories", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_AdminGroups_AdminGroupId1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AdminGroupModules");

            migrationBuilder.DropTable(
                name: "AdminGroups");

            migrationBuilder.DropTable(
                name: "UserLoginHistories");

            migrationBuilder.DropIndex(
                name: "IX_Users_AdminGroupId1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdminGroupId",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdminGroupId1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLoggedIn",
                schema: "Identity",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "RestrictAccessIPAddress",
                schema: "Identity",
                table: "Users",
                newName: "IPAddress");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
