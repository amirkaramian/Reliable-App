using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class MultipleIpsRestrictionForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestrictAccessIPAddress",
                schema: "Identity",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "UserRestrictedIps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RestrictAccessIPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRestrictedIps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRestrictedIps_Users_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalSchema: "Identity",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRestrictedIps_ApplicationUserId",
                table: "UserRestrictedIps",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserRestrictedIps");

            migrationBuilder.AddColumn<string>(
                name: "RestrictAccessIPAddress",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
