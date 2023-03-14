using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class TicketHistoryCommentnReplyIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketCommentId",
                table: "TicketHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TicketCommentReplyId",
                table: "TicketHistories",
                type: "uniqueidentifier",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketCommentId",
                table: "TicketHistories");

            migrationBuilder.DropColumn(
                name: "TicketCommentReplyId",
                table: "TicketHistories");
        }
    }
}
