using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class AdminAsClientLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "UploadFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "TemplateVariables",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "SmtpConfigurations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Settings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Refunds",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "RecurringPayments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "PaymentGateways",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "NotificationTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Modules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Hooks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "FraudAlerts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Feedbacks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "EmailTemplates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "CronJobs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Credit",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Comments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Brands",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Bills",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "BillOrders",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "Articles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AdminAsClient",
                table: "APIKeyPairs",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "TemplateVariables");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "SmtpConfigurations");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "RecurringPayments");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "PaymentGateways");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Hooks");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "FraudAlerts");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "CronJobs");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Credit");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "BillOrders");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "AdminAsClient",
                table: "APIKeyPairs");
        }
    }
}
