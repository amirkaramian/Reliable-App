using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class DefaultVATaddedForOrderInSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultBillDueDate",
                table: "Settings");

            migrationBuilder.AddColumn<int>(
                name: "DefaultBillDueDays",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "VAT",
                table: "Settings",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultBillDueDays",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "VAT",
                table: "Settings");

            migrationBuilder.AddColumn<DateTime>(
                name: "DefaultBillDueDate",
                table: "Settings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
