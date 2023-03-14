using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class TicketHistoryCommentnReplyDtos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketCommentHistoryId",
                table: "TicketHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TicketCommentReplyHistoryId",
                table: "TicketHistories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TicketCommentHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    IsSticky = table.Column<bool>(type: "bit", nullable: false),
                    TicketCommentAction = table.Column<int>(type: "int", nullable: false),
                    TicketCommentType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCommentHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TicketCommentReplyHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketCommentHistoryParentReplyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TicketCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCommentReplyHistories", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketCommentHistories");

            migrationBuilder.DropTable(
                name: "TicketCommentReplyHistories");

            migrationBuilder.DropColumn(
                name: "TicketCommentHistoryId",
                table: "TicketHistories");

            migrationBuilder.DropColumn(
                name: "TicketCommentReplyHistoryId",
                table: "TicketHistories");
        }
    }
}
