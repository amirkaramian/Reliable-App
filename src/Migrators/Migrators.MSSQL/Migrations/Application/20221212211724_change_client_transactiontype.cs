using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class change_client_transactiontype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreditTRansactionType",
                table: "Credit",
                newName: "CreditTransactionType");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreditTransactionType",
                table: "Credit",
                newName: "CreditTRansactionType");
        }
    }
}
