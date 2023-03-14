using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class M2M_BrandSmtp_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrandSmtps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SmtpConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandSmtps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BrandSmtps_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandSmtps_SmtpConfigurations_SmtpConfigurationId",
                        column: x => x.SmtpConfigurationId,
                        principalTable: "SmtpConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandSmtps_BrandId",
                table: "BrandSmtps",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_BrandSmtps_SmtpConfigurationId",
                table: "BrandSmtps",
                column: "SmtpConfigurationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandSmtps");
        }
    }
}
