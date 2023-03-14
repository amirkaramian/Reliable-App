using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class ArticleAndArticleFeedbackRelation_03 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
