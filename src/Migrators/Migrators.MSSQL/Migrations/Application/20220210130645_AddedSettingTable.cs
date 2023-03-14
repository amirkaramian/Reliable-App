using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class AddedSettingTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Settings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                DateFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DefaultCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TermsOfServiceURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TermsOfServiceAgreement = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                RecordsToDisplay = table.Column<int>(type: "int", nullable: false),
                AutoRefreshInterval = table.Column<int>(type: "int", nullable: false),
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
                table.PrimaryKey("PK_Settings", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Settings");
    }
}
