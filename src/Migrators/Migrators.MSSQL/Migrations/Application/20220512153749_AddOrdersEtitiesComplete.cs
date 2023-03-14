using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class AddOrdersEtitiesComplete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Transactions",
                newName: "TransactionBy");

            migrationBuilder.AddColumn<int>(
                name: "ReferenceId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TransactionNo",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BillPrefix",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DefaultBillDueDate",
                table: "Settings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionNo",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BillPrefix",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "DefaultBillDueDate",
                table: "Settings");

            migrationBuilder.RenameColumn(
                name: "TransactionBy",
                table: "Transactions",
                newName: "Description");

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "Transactions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
