using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class AddCanTakeOrdersCanTakeTicketsForUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCategories_Articles_ArticleId",
                table: "ArticleCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentReplies_TicketComments_TicketCommentId",
                table: "TicketCommentReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments");

            migrationBuilder.AddColumn<bool>(
                name: "CanTakeOrders",
                schema: "Identity",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanTakeTickets",
                schema: "Identity",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketId",
                table: "TicketComments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketCommentId",
                table: "TicketCommentReplies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "ArticleCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCategories_Articles_ArticleId",
                table: "ArticleCategories",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentReplies_TicketComments_TicketCommentId",
                table: "TicketCommentReplies",
                column: "TicketCommentId",
                principalTable: "TicketComments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCategories_Articles_ArticleId",
                table: "ArticleCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentReplies_TicketComments_TicketCommentId",
                table: "TicketCommentReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "CanTakeOrders",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CanTakeTickets",
                schema: "Identity",
                table: "Users");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketId",
                table: "TicketComments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "TicketCommentId",
                table: "TicketCommentReplies",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArticleId",
                table: "ArticleCategories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCategories_Articles_ArticleId",
                table: "ArticleCategories",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentReplies_TicketComments_TicketCommentId",
                table: "TicketCommentReplies",
                column: "TicketCommentId",
                principalTable: "TicketComments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }
    }
}
