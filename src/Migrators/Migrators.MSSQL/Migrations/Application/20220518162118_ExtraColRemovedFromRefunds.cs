using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class ExtraColRemovedFromRefunds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundRetainPercentage",
                table: "Refunds");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RefundRetainPercentage",
                table: "Refunds",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
