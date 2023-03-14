using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class AddAppKeyPair : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "APIKeyPairs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                ApplicationKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserIds = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SafeListIpAddresses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Permissions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ValidTill = table.Column<DateTime>(type: "datetime2", nullable: false),
                StatusApi = table.Column<bool>(type: "bit", nullable: false),
                Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_APIKeyPairs", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "APIKeyPairs");
    }
}