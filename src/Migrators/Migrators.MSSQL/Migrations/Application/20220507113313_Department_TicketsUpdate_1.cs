using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Department_TicketsUpdate_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketComment_Tickets_TicketId",
                table: "TicketComment");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentReply_TicketComment_TicketCommentId",
                table: "TicketCommentReply");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentReply_TicketCommentReply_TicketCommentReplyId",
                table: "TicketCommentReply");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketCommentReply",
                table: "TicketCommentReply");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketComment",
                table: "TicketComment");

            migrationBuilder.RenameTable(
                name: "TicketCommentReply",
                newName: "TicketCommentReplies");

            migrationBuilder.RenameTable(
                name: "TicketComment",
                newName: "TicketComments");

            migrationBuilder.RenameIndex(
                name: "IX_TicketCommentReply_TicketCommentReplyId",
                table: "TicketCommentReplies",
                newName: "IX_TicketCommentReplies_TicketCommentReplyId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketCommentReply_TicketCommentId",
                table: "TicketCommentReplies",
                newName: "IX_TicketCommentReplies_TicketCommentId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketComment_TicketId",
                table: "TicketComments",
                newName: "IX_TicketComments_TicketId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketCommentReplies",
                table: "TicketCommentReplies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketComments",
                table: "TicketComments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentReplies_TicketCommentReplies_TicketCommentReplyId",
                table: "TicketCommentReplies",
                column: "TicketCommentReplyId",
                principalTable: "TicketCommentReplies",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentReplies_TicketCommentReplies_TicketCommentReplyId",
                table: "TicketCommentReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketCommentReplies_TicketComments_TicketCommentId",
                table: "TicketCommentReplies");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketComments",
                table: "TicketComments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TicketCommentReplies",
                table: "TicketCommentReplies");

            migrationBuilder.RenameTable(
                name: "TicketComments",
                newName: "TicketComment");

            migrationBuilder.RenameTable(
                name: "TicketCommentReplies",
                newName: "TicketCommentReply");

            migrationBuilder.RenameIndex(
                name: "IX_TicketComments_TicketId",
                table: "TicketComment",
                newName: "IX_TicketComment_TicketId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketCommentReplies_TicketCommentReplyId",
                table: "TicketCommentReply",
                newName: "IX_TicketCommentReply_TicketCommentReplyId");

            migrationBuilder.RenameIndex(
                name: "IX_TicketCommentReplies_TicketCommentId",
                table: "TicketCommentReply",
                newName: "IX_TicketCommentReply_TicketCommentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketComment",
                table: "TicketComment",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TicketCommentReply",
                table: "TicketCommentReply",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComment_Tickets_TicketId",
                table: "TicketComment",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentReply_TicketComment_TicketCommentId",
                table: "TicketCommentReply",
                column: "TicketCommentId",
                principalTable: "TicketComment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketCommentReply_TicketCommentReply_TicketCommentReplyId",
                table: "TicketCommentReply",
                column: "TicketCommentReplyId",
                principalTable: "TicketCommentReply",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }
    }
}
