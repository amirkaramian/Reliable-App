using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class OrderTemplates_Add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BilledProductLineItems_Bills_BillId",
                table: "BilledProductLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Bills_Products_ProductId",
                table: "Bills");

            migrationBuilder.DropIndex(
                name: "IX_Bills_ProductId",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Bills");

            migrationBuilder.RenameColumn(
                name: "ChildCategoryIcon",
                table: "Categories",
                newName: "CategoryIcon");

            migrationBuilder.RenameColumn(
                name: "BillId",
                table: "BilledProductLineItems",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_BilledProductLineItems_BillId",
                table: "BilledProductLineItems",
                newName: "IX_BilledProductLineItems_OrderId");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderTemplateId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNo",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderTemplateId",
                table: "BilledProductLineItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminAsClient = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderTemplates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrderId",
                table: "Products",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrderTemplateId",
                table: "Products",
                column: "OrderTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_BilledProductLineItems_OrderTemplateId",
                table: "BilledProductLineItems",
                column: "OrderTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_BilledProductLineItems_Orders_OrderId",
                table: "BilledProductLineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_BilledProductLineItems_OrderTemplates_OrderTemplateId",
                table: "BilledProductLineItems",
                column: "OrderTemplateId",
                principalTable: "OrderTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_OrderTemplates_OrderTemplateId",
                table: "Products",
                column: "OrderTemplateId",
                principalTable: "OrderTemplates",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BilledProductLineItems_Orders_OrderId",
                table: "BilledProductLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_BilledProductLineItems_OrderTemplates_OrderTemplateId",
                table: "BilledProductLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Orders_OrderId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_OrderTemplates_OrderTemplateId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "OrderTemplates");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrderId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrderTemplateId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_BilledProductLineItems_OrderTemplateId",
                table: "BilledProductLineItems");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrderTemplateId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InvoiceNo",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "OrderTemplateId",
                table: "BilledProductLineItems");

            migrationBuilder.RenameColumn(
                name: "CategoryIcon",
                table: "Categories",
                newName: "ChildCategoryIcon");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "BilledProductLineItems",
                newName: "BillId");

            migrationBuilder.RenameIndex(
                name: "IX_BilledProductLineItems_OrderId",
                table: "BilledProductLineItems",
                newName: "IX_BilledProductLineItems_BillId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "Bills",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bills_ProductId",
                table: "Bills",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_BilledProductLineItems_Bills_BillId",
                table: "BilledProductLineItems",
                column: "BillId",
                principalTable: "Bills",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Products_ProductId",
                table: "Bills",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
