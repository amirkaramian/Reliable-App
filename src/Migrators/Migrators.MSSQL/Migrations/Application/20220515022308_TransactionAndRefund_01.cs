using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class TransactionAndRefund_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Transactions",
                newName: "TransactionStatus");

            migrationBuilder.AddColumn<string>(
                name: "ActionTakenBy",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionByRole",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RefundStatus",
                table: "Orders",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionTakenBy",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransactionByRole",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RefundStatus",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TransactionStatus",
                table: "Transactions",
                newName: "Status");
        }
    }
}
