using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class NotificationsAndBillsRelatedTbls : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Level",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Read",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "NotificationTemplates",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "SendAt",
                table: "Notifications",
                newName: "SentAt");

            migrationBuilder.RenameColumn(
                name: "ForUser",
                table: "Notifications",
                newName: "ToUserId");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Notifications",
                newName: "Body");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "NotificationTemplates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "NotificationTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "NotificationTemplates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NotificationTemplateNo",
                table: "NotificationTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "OperatorType",
                table: "NotificationTemplates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Property",
                table: "NotificationTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "NotificationTemplates",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "NotificationTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TargetUserType",
                table: "NotificationTemplates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TargetUserTypes",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "Bills",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "NotificationTemplateNo",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "OperatorType",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "Property",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "TargetUserType",
                table: "NotificationTemplates");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TargetUserTypes",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "Bills");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "NotificationTemplates",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "ToUserId",
                table: "Notifications",
                newName: "ForUser");

            migrationBuilder.RenameColumn(
                name: "SentAt",
                table: "Notifications",
                newName: "SendAt");

            migrationBuilder.RenameColumn(
                name: "Body",
                table: "Notifications",
                newName: "Description");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "NotificationTemplates",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "Level",
                table: "Notifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "Read",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
