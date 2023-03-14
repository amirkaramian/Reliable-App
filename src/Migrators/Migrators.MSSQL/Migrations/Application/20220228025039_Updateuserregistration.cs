using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Updateuserregistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address1",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentID",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State_Region",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Address2",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Country",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ParentID",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "State_Region",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ZipCode",
                schema: "Identity",
                table: "Users");
        }
    }
}
