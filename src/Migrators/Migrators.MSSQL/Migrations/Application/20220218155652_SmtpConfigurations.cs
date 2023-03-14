using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class SmtpConfigurations : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "SmtpConfigurations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Port = table.Column<int>(type: "int", nullable: false),
                HttpsProtocol = table.Column<bool>(type: "bit", nullable: false),
                Host = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FromName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FromEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Signature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CssStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                HeaderContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FooterContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CompanyAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Bcc = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                table.PrimaryKey("PK_SmtpConfigurations", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "SmtpConfigurations");
    }
}