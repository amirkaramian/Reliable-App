using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Change_Credit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoBill",
                table: "UserAppSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Credit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Credit",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "MaxCreditAmount",
                table: "BillingSettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoBill",
                table: "UserAppSettings");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Credit");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Credit");

            migrationBuilder.DropColumn(
                name: "MaxCreditAmount",
                table: "BillingSettings");
        }
    }
}
