using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class BrandArticles_0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandArtilce_Articles_Articleid",
                table: "BrandArtilce");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandArtilce_Brands_BrandId",
                table: "BrandArtilce");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BrandArtilce",
                table: "BrandArtilce");

            migrationBuilder.RenameTable(
                name: "BrandArtilce",
                newName: "BrandArtilces");

            migrationBuilder.RenameIndex(
                name: "IX_BrandArtilce_BrandId",
                table: "BrandArtilces",
                newName: "IX_BrandArtilces_BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_BrandArtilce_Articleid",
                table: "BrandArtilces",
                newName: "IX_BrandArtilces_Articleid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BrandArtilces",
                table: "BrandArtilces",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandArtilces_Articles_Articleid",
                table: "BrandArtilces",
                column: "Articleid",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandArtilces_Brands_BrandId",
                table: "BrandArtilces",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BrandArtilces_Articles_Articleid",
                table: "BrandArtilces");

            migrationBuilder.DropForeignKey(
                name: "FK_BrandArtilces_Brands_BrandId",
                table: "BrandArtilces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BrandArtilces",
                table: "BrandArtilces");

            migrationBuilder.RenameTable(
                name: "BrandArtilces",
                newName: "BrandArtilce");

            migrationBuilder.RenameIndex(
                name: "IX_BrandArtilces_BrandId",
                table: "BrandArtilce",
                newName: "IX_BrandArtilce_BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_BrandArtilces_Articleid",
                table: "BrandArtilce",
                newName: "IX_BrandArtilce_Articleid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BrandArtilce",
                table: "BrandArtilce",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BrandArtilce_Articles_Articleid",
                table: "BrandArtilce",
                column: "Articleid",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BrandArtilce_Brands_BrandId",
                table: "BrandArtilce",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
