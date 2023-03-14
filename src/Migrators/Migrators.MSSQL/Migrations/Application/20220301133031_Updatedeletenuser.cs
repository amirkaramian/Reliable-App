using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application;

public partial class Updatedeletenuser : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {

        migrationBuilder.AddColumn<bool>(
            name: "IsDeleted",
            schema: "Identity",
            table: "Users",
            type: "bit",
            nullable: false,
            defaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsDeleted",
            schema: "Identity",
            table: "Users");
    }
}
