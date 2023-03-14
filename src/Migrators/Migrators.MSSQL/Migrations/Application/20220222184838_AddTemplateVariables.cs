using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class AddTemplateVariables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "UserId",
            table: "SmtpConfigurations");

        migrationBuilder.DropColumn(
            name: "UserId",
            table: "Notifications");

        migrationBuilder.DropColumn(
            name: "UserId",
            table: "EmailTemplates");

        migrationBuilder.DropColumn(
            name: "UserId",
            table: "Brands");

        migrationBuilder.CreateTable(
            name: "TemplateVariables",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Variable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                table.PrimaryKey("PK_TemplateVariables", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "TemplateVariables");

        migrationBuilder.AddColumn<Guid>(
            name: "UserId",
            table: "SmtpConfigurations",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "UserId",
            table: "Notifications",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "UserId",
            table: "EmailTemplates",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "UserId",
            table: "Brands",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
    }
}