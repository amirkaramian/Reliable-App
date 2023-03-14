using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class ArticleAndArticleFeedbackRelation_02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleFeedbacks_ArticleFeedbackId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleFeedbackId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleFeedbackId",
                table: "Articles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ArticleFeedbackId",
                table: "Articles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleFeedbackId",
                table: "Articles",
                column: "ArticleFeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleFeedbacks_ArticleFeedbackId",
                table: "Articles",
                column: "ArticleFeedbackId",
                principalTable: "ArticleFeedbacks",
                principalColumn: "Id");
        }
    }
}
