using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class BrandArticles_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandArtilces");

            migrationBuilder.CreateTable(
                name: "BrandArticles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Articleid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandArticles_Articles_Articleid",
                        column: x => x.Articleid,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandArticles_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandArticles_Articleid",
                table: "BrandArticles",
                column: "Articleid");

            migrationBuilder.CreateIndex(
                name: "IX_BrandArticles_BrandId",
                table: "BrandArticles",
                column: "BrandId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandArticles");

            migrationBuilder.CreateTable(
                name: "BrandArtilces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Articleid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandArtilces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandArtilces_Articles_Articleid",
                        column: x => x.Articleid,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandArtilces_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandArtilces_Articleid",
                table: "BrandArtilces",
                column: "Articleid");

            migrationBuilder.CreateIndex(
                name: "IX_BrandArtilces_BrandId",
                table: "BrandArtilces",
                column: "BrandId");
        }
    }
}
