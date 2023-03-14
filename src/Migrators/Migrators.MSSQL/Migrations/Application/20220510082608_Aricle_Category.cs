using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Aricle_Category : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Rate",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Articles",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Articles",
                newName: "BodyText");

            migrationBuilder.AddColumn<Guid>(
                name: "ArticleId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryType",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ChildCategoryIcon",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCategoryId",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ArticleFeedbackId",
                table: "Articles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ArticleStatus",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Visibility",
                table: "Articles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ArticleId",
                table: "Categories",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ArticleFeedbackId",
                table: "Articles",
                column: "ArticleFeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Tickets_ArticleFeedbackId",
                table: "Articles",
                column: "ArticleFeedbackId",
                principalTable: "Tickets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Articles_ArticleId",
                table: "Categories",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Tickets_ArticleFeedbackId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Articles_ArticleId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ArticleId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ArticleFeedbackId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryType",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ChildCategoryIcon",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ArticleFeedbackId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "ArticleStatus",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "Visibility",
                table: "Articles");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Articles",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "BodyText",
                table: "Articles",
                newName: "Description");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Articles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "Rate",
                table: "Articles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                table: "Articles",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                table: "Articles",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
