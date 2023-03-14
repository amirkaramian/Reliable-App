using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Department_TicketsUpdate_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentReplies_TicketCommentReplies_TicketCommentReplyId",
                table: "TicketCommentReplies");

            migrationBuilder.DropIndex(
                name: "IX_TicketCommentReplies_TicketCommentReplyId",
                table: "TicketCommentReplies");

            migrationBuilder.DropColumn(
                name: "TicketCommentReplyId",
                table: "TicketCommentReplies");

            migrationBuilder.AddColumn<Guid>(
                name: "TicketCommentParentReplyId",
                table: "TicketCommentReplies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentReplies_TicketCommentParentReplyId",
                table: "TicketCommentReplies",
                column: "TicketCommentParentReplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentReplies_TicketCommentReplies_TicketCommentParentReplyId",
                table: "TicketCommentReplies",
                column: "TicketCommentParentReplyId",
                principalTable: "TicketCommentReplies",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentReplies_TicketCommentReplies_TicketCommentParentReplyId",
                table: "TicketCommentReplies");

            migrationBuilder.DropIndex(
                name: "IX_TicketCommentReplies_TicketCommentParentReplyId",
                table: "TicketCommentReplies");

            migrationBuilder.DropColumn(
                name: "TicketCommentParentReplyId",
                table: "TicketCommentReplies");

            migrationBuilder.AddColumn<Guid>(
                name: "TicketCommentReplyId",
                table: "TicketCommentReplies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentReplies_TicketCommentReplyId",
                table: "TicketCommentReplies",
                column: "TicketCommentReplyId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentReplies_TicketCommentReplies_TicketCommentReplyId",
                table: "TicketCommentReplies",
                column: "TicketCommentReplyId",
                principalTable: "TicketCommentReplies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
