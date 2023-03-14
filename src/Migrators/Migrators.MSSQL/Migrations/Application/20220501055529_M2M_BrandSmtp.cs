using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class M2M_BrandSmtp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SmtpConfigurationId",
                table: "Brands",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BrandSmtpConfiguration",
                columns: table => new
                {
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SmtpConfigurationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrandSmtpConfiguration", x => new { x.BrandId, x.SmtpConfigurationId });
                    table.ForeignKey(
                        name: "FK_BrandSmtpConfiguration_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BrandSmtpConfiguration_SmtpConfigurations_SmtpConfigurationId",
                        column: x => x.SmtpConfigurationId,
                        principalTable: "SmtpConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BrandSmtpConfiguration_SmtpConfigurationId",
                table: "BrandSmtpConfiguration",
                column: "SmtpConfigurationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrandSmtpConfiguration");

            migrationBuilder.DropColumn(
                name: "SmtpConfigurationId",
                table: "Brands");
        }
    }
}
