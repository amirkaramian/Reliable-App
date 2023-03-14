using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class OrderTemplates_Add_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BilledProductLineItems_Orders_OrderId",
                table: "BilledProductLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BilledProductLineItems_OrderTemplates_OrderTemplateId",
                table: "BilledProductLineItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BilledProductLineItems",
                table: "BilledProductLineItems");

            migrationBuilder.RenameTable(
                name: "BilledProductLineItems",
                newName: "OrderProductLineItems");

            migrationBuilder.RenameIndex(
                name: "IX_BilledProductLineItems_OrderTemplateId",
                table: "OrderProductLineItems",
                newName: "IX_OrderProductLineItems_OrderTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_BilledProductLineItems_OrderId",
                table: "OrderProductLineItems",
                newName: "IX_OrderProductLineItems_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderProductLineItems",
                table: "OrderProductLineItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductLineItems_Orders_OrderId",
                table: "OrderProductLineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductLineItems_OrderTemplates_OrderTemplateId",
                table: "OrderProductLineItems",
                column: "OrderTemplateId",
                principalTable: "OrderTemplates",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductLineItems_Orders_OrderId",
                table: "OrderProductLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductLineItems_OrderTemplates_OrderTemplateId",
                table: "OrderProductLineItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderProductLineItems",
                table: "OrderProductLineItems");

            migrationBuilder.RenameTable(
                name: "OrderProductLineItems",
                newName: "BilledProductLineItems");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProductLineItems_OrderTemplateId",
                table: "BilledProductLineItems",
                newName: "IX_BilledProductLineItems_OrderTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProductLineItems_OrderId",
                table: "BilledProductLineItems",
                newName: "IX_BilledProductLineItems_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BilledProductLineItems",
                table: "BilledProductLineItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BilledProductLineItems_Orders_OrderId",
                table: "BilledProductLineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BilledProductLineItems_OrderTemplates_OrderTemplateId",
                table: "BilledProductLineItems",
                column: "OrderTemplateId",
                principalTable: "OrderTemplates",
                principalColumn: "Id");
        }
    }
}
