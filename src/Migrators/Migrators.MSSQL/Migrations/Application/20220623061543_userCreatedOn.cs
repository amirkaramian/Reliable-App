using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class userCreatedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Brands_BrandId1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BrandId1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BrandId1",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "Identity",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "BrandId1",
                schema: "Identity",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_BrandId1",
                schema: "Identity",
                table: "Users",
                column: "BrandId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Brands_BrandId1",
                schema: "Identity",
                table: "Users",
                column: "BrandId1",
                principalTable: "Brands",
                principalColumn: "Id");
        }
    }
}
