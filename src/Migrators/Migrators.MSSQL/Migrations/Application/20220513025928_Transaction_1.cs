using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Transaction_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReferenceNumber",
                table: "Transactions",
                newName: "ReferenceNo");

            migrationBuilder.AddColumn<Guid>(
                name: "ReferenceId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "ReferenceNo",
                table: "Transactions",
                newName: "ReferenceNumber");
        }
    }
}
