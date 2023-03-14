using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class updOrdernProd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "ClientId");

            migrationBuilder.AddColumn<string>(
                name: "AdminAssigned",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminAssigned",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ClientId",
                table: "Orders",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
