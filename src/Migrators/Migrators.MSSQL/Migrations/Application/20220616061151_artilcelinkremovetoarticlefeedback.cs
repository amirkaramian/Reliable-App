using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class artilcelinkremovetoarticlefeedback : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleFeedbacks_Articles_ArticleId",
                table: "ArticleFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_ArticleFeedbacks_ArticleId",
                table: "ArticleFeedbacks");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "ArticleFeedbacks");

            migrationBuilder.AddColumn<Guid>(
                name: "ArticleFeedbacksId",
                table: "Articles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleFeedbacksId",
                table: "Articles",
                column: "ArticleFeedbacksId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleFeedbacks_ArticleFeedbacksId",
                table: "Articles",
                column: "ArticleFeedbacksId",
                principalTable: "ArticleFeedbacks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleFeedbacks_ArticleFeedbacksId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleFeedbacksId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleFeedbacksId",
                table: "Articles");

            migrationBuilder.AddColumn<Guid>(
                name: "ArticleId",
                table: "ArticleFeedbacks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ArticleFeedbacks_ArticleId",
                table: "ArticleFeedbacks",
                column: "ArticleId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleFeedbacks_Articles_ArticleId",
                table: "ArticleFeedbacks",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
