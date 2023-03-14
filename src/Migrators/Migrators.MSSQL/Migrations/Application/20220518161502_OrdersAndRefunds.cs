using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class OrdersAndRefunds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefundDate",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "RefundStatus",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "RefundCustId",
                table: "Refunds",
                newName: "RequestById");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Refunds",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "BillOrderId",
                table: "Refunds",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Refunds",
                newName: "TotalAfterRetainPercentage");

            migrationBuilder.AddColumn<Guid>(
                name: "ActionTakenById",
                table: "Refunds",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefundNo",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<decimal>(
                name: "RefundRetainPercentage",
                table: "Refunds",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RefundStatus",
                table: "Refunds",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Refunds",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductNo",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_OrderId",
                table: "Refunds",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Orders_OrderId",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_OrderId",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "ActionTakenById",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "RefundNo",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "RefundRetainPercentage",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "RefundStatus",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "ProductNo",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "TotalAfterRetainPercentage",
                table: "Refunds",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "RequestById",
                table: "Refunds",
                newName: "RefundCustId");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Refunds",
                newName: "BillOrderId");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Refunds",
                newName: "Description");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundDate",
                table: "Refunds",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "RefundStatus",
                table: "Orders",
                type: "int",
                nullable: true);
        }
    }
}
