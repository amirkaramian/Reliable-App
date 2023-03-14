using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class Tickets_FollowUp_history_Upd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentUser",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BrandId",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BrandId1",
                table: "Tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientEmail",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientFullName",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientId",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FollowUpComment",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FollowUpOn",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdleTime",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PinTicket",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PriorityFollowUp",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketNumber",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "TransferComments",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransferOn",
                table: "Tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSticky",
                table: "TicketComments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TicketHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketNumber = table.Column<int>(type: "int", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketStatus = table.Column<int>(type: "int", nullable: false),
                    TicketPriority = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketRelatedTo = table.Column<int>(type: "int", nullable: false),
                    TicketRelatedToId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrandId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrandId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdleTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinTicket = table.Column<bool>(type: "bit", nullable: false),
                    FollowUpOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Group = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgentUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriorityFollowUp = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FollowUpComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActionBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedToFullName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketHistories_Brands_BrandId1",
                        column: x => x.BrandId1,
                        principalTable: "Brands",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_BrandId1",
                table: "Tickets",
                column: "BrandId1");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistories_BrandId1",
                table: "TicketHistories",
                column: "BrandId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Brands_BrandId1",
                table: "Tickets",
                column: "BrandId1",
                principalTable: "Brands",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Brands_BrandId1",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "TicketHistories");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_BrandId1",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "AgentUser",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BrandId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "BrandId1",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ClientEmail",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ClientFullName",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FollowUpComment",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "FollowUpOn",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IdleTime",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PinTicket",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "PriorityFollowUp",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TicketNumber",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TransferComments",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "TransferOn",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsSticky",
                table: "TicketComments");
        }
    }
}
