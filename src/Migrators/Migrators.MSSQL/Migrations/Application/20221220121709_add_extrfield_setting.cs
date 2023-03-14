using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class add_extrfield_setting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "IsActiveOrPendingProducts",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxCreditAmount",
                table: "Settings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActiveOrPendingProducts",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "MaxCreditAmount",
                table: "Settings");
        }
    }
}
