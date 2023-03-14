using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class whmcsprodorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssginedIPs",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DedicatedIP",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DomainName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldOrderId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldProductId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServerId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssginedIPs",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DedicatedIP",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DomainName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OldOrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OldProductId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ServerId",
                table: "Products");
        }
    }
}
