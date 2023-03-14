using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class EmailNotifications : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Brands",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LogoUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                table.PrimaryKey("PK_Brands", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "EmailTemplates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmailTemplates", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                ForUser = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Level = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                SendAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Read = table.Column<bool>(type: "bit", nullable: false),
                Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "NotificationTemplates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NotificationTemplates", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Brands");

        migrationBuilder.DropTable(
            name: "EmailTemplates");

        migrationBuilder.DropTable(
            name: "Notifications");

        migrationBuilder.DropTable(
            name: "NotificationTemplates");
    }
}