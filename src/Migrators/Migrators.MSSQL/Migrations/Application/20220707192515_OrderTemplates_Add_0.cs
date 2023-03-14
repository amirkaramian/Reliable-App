using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class OrderTemplates_Add_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BilledProductLineItems_Orders_OrderId",
                table: "BilledProductLineItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "BilledProductLineItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "BilledProductLineItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProductLineItemId",
                table: "BilledProductLineItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_BilledProductLineItems_Orders_OrderId",
                table: "BilledProductLineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BilledProductLineItems_Orders_OrderId",
                table: "BilledProductLineItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "BilledProductLineItems");

            migrationBuilder.DropColumn(
                name: "ProductLineItemId",
                table: "BilledProductLineItems");

            migrationBuilder.AlterColumn<Guid>(
                name: "OrderId",
                table: "BilledProductLineItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BilledProductLineItems_Orders_OrderId",
                table: "BilledProductLineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
