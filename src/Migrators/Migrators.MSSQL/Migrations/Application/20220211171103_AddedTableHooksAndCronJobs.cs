using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class AddedTableHooksAndCronJobs : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "CronJobs",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OwnerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RunTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
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
                table.PrimaryKey("PK_CronJobs", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Hooks",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OwnerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                RunTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Status = table.Column<int>(type: "int", nullable: false),
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
                table.PrimaryKey("PK_Hooks", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CronJobs");

        migrationBuilder.DropTable(
            name: "Hooks");
    }
}