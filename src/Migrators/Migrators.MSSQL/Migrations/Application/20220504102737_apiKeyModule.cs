using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class apiKeyModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "APIKeyPairs");

            migrationBuilder.CreateTable(
                name: "UserApiKeyModule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermissionDetail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    APIKeyPairId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_UserApiKeyModule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserApiKeyModule_APIKeyPairs_APIKeyPairId",
                        column: x => x.APIKeyPairId,
                        principalTable: "APIKeyPairs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserApiKeyModule_APIKeyPairId",
                table: "UserApiKeyModule",
                column: "APIKeyPairId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserApiKeyModule");

            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "APIKeyPairs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
