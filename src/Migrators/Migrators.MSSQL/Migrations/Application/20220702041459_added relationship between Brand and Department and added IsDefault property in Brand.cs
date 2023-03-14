using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class addedrelationshipbetweenBrandandDepartmentandaddedIsDefaultpropertyinBrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BrandId",
                table: "Departments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Brands",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_BrandId",
                table: "Departments",
                column: "BrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Brands_BrandId",
                table: "Departments",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Brands_BrandId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_BrandId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Brands");
        }
    }
}
