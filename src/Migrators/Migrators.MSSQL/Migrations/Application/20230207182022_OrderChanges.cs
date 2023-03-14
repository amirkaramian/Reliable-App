using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class OrderChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdminAssigned",
                table: "Orders",
                newName: "AdminAssignedString");

            migrationBuilder.AddColumn<bool>(
                name: "Notify",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notify",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "AdminAssignedString",
                table: "Orders",
                newName: "AdminAssigned");
        }
    }
}
