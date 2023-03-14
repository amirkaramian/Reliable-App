using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Department_TicketsUpdate_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentIds",
                schema: "Identity",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "TicketComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_TicketComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketComment_Tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "Tickets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TicketCommentReply",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketCommentReplyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.PrimaryKey("PK_TicketCommentReply", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCommentReply_TicketComment_TicketCommentId",
                        column: x => x.TicketCommentId,
                        principalTable: "TicketComment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TicketCommentReply_TicketCommentReply_TicketCommentReplyId",
                        column: x => x.TicketCommentReplyId,
                        principalTable: "TicketCommentReply",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketComment_TicketId",
                table: "TicketComment",
                column: "TicketId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentReply_TicketCommentId",
                table: "TicketCommentReply",
                column: "TicketCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCommentReply_TicketCommentReplyId",
                table: "TicketCommentReply",
                column: "TicketCommentReplyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TicketCommentReply");

            migrationBuilder.DropTable(
                name: "TicketComment");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentIds",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
