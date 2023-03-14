using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class OTUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProductLineItems_OrderTemplates_OrderTemplateId",
                table: "OrderProductLineItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_OrderTemplates_OrderTemplateId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_OrderTemplateId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_OrderProductLineItems_OrderTemplateId",
                table: "OrderProductLineItems");

            migrationBuilder.DropColumn(
                name: "OrderTemplateId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OrderTemplateId",
                table: "OrderProductLineItems");

            migrationBuilder.RenameColumn(
                name: "TemplateName",
                table: "OrderTemplates",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "OrderTemplates",
                newName: "Thumbnail");

            migrationBuilder.AddColumn<int>(
                name: "BillingCycle",
                table: "OrderTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OrderTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "OrderTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrderTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "OrderTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OrderTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "OrderTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderTemplateCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_OrderTemplateCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTemplateCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTemplateCategories_OrderTemplates_OrderTemplateId",
                        column: x => x.OrderTemplateId,
                        principalTable: "OrderTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTemplateDepartments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_OrderTemplateDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTemplateDepartments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderTemplateDepartments_OrderTemplates_OrderTemplateId",
                        column: x => x.OrderTemplateId,
                        principalTable: "OrderTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderTemplateLineItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineItem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceType = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_OrderTemplateLineItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderTemplateLineItems_OrderTemplates_OrderTemplateId",
                        column: x => x.OrderTemplateId,
                        principalTable: "OrderTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderTemplateCategories_CategoryId",
                table: "OrderTemplateCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTemplateCategories_OrderTemplateId",
                table: "OrderTemplateCategories",
                column: "OrderTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTemplateDepartments_DepartmentId",
                table: "OrderTemplateDepartments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTemplateDepartments_OrderTemplateId",
                table: "OrderTemplateDepartments",
                column: "OrderTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderTemplateLineItems_OrderTemplateId",
                table: "OrderTemplateLineItems",
                column: "OrderTemplateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderTemplateCategories");

            migrationBuilder.DropTable(
                name: "OrderTemplateDepartments");

            migrationBuilder.DropTable(
                name: "OrderTemplateLineItems");

            migrationBuilder.DropColumn(
                name: "BillingCycle",
                table: "OrderTemplates");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "OrderTemplates");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "OrderTemplates");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderTemplates");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "OrderTemplates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrderTemplates");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "OrderTemplates");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "OrderTemplates",
                newName: "TemplateName");

            migrationBuilder.RenameColumn(
                name: "Thumbnail",
                table: "OrderTemplates",
                newName: "Notes");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderTemplateId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrderTemplateId",
                table: "OrderProductLineItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_OrderTemplateId",
                table: "Products",
                column: "OrderTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductLineItems_OrderTemplateId",
                table: "OrderProductLineItems",
                column: "OrderTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProductLineItems_OrderTemplates_OrderTemplateId",
                table: "OrderProductLineItems",
                column: "OrderTemplateId",
                principalTable: "OrderTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_OrderTemplates_OrderTemplateId",
                table: "Products",
                column: "OrderTemplateId",
                principalTable: "OrderTemplates",
                principalColumn: "Id");
        }
    }
}
