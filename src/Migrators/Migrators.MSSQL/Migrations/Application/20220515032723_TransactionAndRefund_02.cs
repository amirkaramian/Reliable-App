using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class TransactionAndRefund_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RefundRetainPercentage",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundRetainPercentage",
                table: "BillingSettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundRetainPercentage",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RefundRetainPercentage",
                table: "BillingSettings");
        }
    }
}
