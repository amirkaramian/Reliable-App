using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class usersettingUpdts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RecordsToDisplay",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "TermsOfServiceAgreement",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "TermsOfServiceURL",
                table: "Settings");

            migrationBuilder.AddColumn<int>(
                name: "RecordsToDisplay",
                table: "UserAppSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TermsOfServiceAgreement",
                table: "Brands",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TermsOfServiceURL",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RecordsToDisplay",
                table: "UserAppSettings");

            migrationBuilder.DropColumn(
                name: "TermsOfServiceAgreement",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "TermsOfServiceURL",
                table: "Brands");

            migrationBuilder.AddColumn<int>(
                name: "RecordsToDisplay",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TermsOfServiceAgreement",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TermsOfServiceURL",
                table: "Settings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }
    }
}
