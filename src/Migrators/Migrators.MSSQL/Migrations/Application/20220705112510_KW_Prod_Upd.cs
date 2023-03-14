using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class KW_Prod_Upd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "ProductLineItems",
                newName: "LineItem");

            migrationBuilder.AddColumn<int>(
                name: "BillingCycle",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PriceType",
                table: "ProductLineItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "ArticleFeedbacks",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingCycle",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PriceType",
                table: "ProductLineItems");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "ArticleFeedbacks");

            migrationBuilder.RenameColumn(
                name: "LineItem",
                table: "ProductLineItems",
                newName: "Name");
        }
    }
}
