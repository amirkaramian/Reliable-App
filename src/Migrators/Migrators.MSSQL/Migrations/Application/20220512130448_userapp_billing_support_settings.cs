using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrators.MSSQL.Migrations.Application
{
    public partial class userapp_billing_support_settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Tickets_ArticleFeedbackId",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "MaxNumberOfRefunds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "MinOrderAmount",
                table: "Settings");

            migrationBuilder.CreateTable(
                name: "ArticleFeedbacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArticleFeedbackStatus = table.Column<int>(type: "int", nullable: false),
                    ArticleFeedbackPriority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArticleFeedbackRelatedTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArticleFeedbackRelatedToId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminAsClient = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleFeedbacks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BillingSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxNumberOfRefunds = table.Column<int>(type: "int", nullable: false),
                    MinOrderAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminAsClient = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaxNumberOfSubCategories = table.Column<int>(type: "int", nullable: false),
                    AutoApproveNewArticles = table.Column<bool>(type: "bit", nullable: false),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminAsClient = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAppSettingss",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientStatus = table.Column<bool>(type: "bit", nullable: false),
                    RequestPerIPOverride = table.Column<int>(type: "int", nullable: false),
                    IPRestrictionIntervalOverrideInSeconds = table.Column<int>(type: "int", nullable: false),
                    APIKeyLimitOverride = table.Column<int>(type: "int", nullable: false),
                    APIKeyIntervalOverrideInSeconds = table.Column<int>(type: "int", nullable: false),
                    RestrictAccessToIPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtendSuspensionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tenant = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminAsClient = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAppSettingss", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArticleFeedbackComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleFeedbackId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminAsClient = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleFeedbackComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleFeedbackComments_ArticleFeedbacks_ArticleFeedbackId",
                        column: x => x.ArticleFeedbackId,
                        principalTable: "ArticleFeedbacks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArticleFeedbackCommentReplies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommentText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ArticleFeedbackCommentParentReplyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArticleFeedbackCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AdminAsClient = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleFeedbackCommentReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArticleFeedbackCommentReplies_ArticleFeedbackCommentReplies_ArticleFeedbackCommentParentReplyId",
                        column: x => x.ArticleFeedbackCommentParentReplyId,
                        principalTable: "ArticleFeedbackCommentReplies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ArticleFeedbackCommentReplies_ArticleFeedbackComments_ArticleFeedbackCommentId",
                        column: x => x.ArticleFeedbackCommentId,
                        principalTable: "ArticleFeedbackComments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleFeedbackCommentReplies_ArticleFeedbackCommentId",
                table: "ArticleFeedbackCommentReplies",
                column: "ArticleFeedbackCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleFeedbackCommentReplies_ArticleFeedbackCommentParentReplyId",
                table: "ArticleFeedbackCommentReplies",
                column: "ArticleFeedbackCommentParentReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleFeedbackComments_ArticleFeedbackId",
                table: "ArticleFeedbackComments",
                column: "ArticleFeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_ArticleFeedbacks_ArticleFeedbackId",
                table: "Articles",
                column: "ArticleFeedbackId",
                principalTable: "ArticleFeedbacks",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_ArticleFeedbacks_ArticleFeedbackId",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "ArticleFeedbackCommentReplies");

            migrationBuilder.DropTable(
                name: "BillingSettings");

            migrationBuilder.DropTable(
                name: "SupportSettings");

            migrationBuilder.DropTable(
                name: "UserAppSettingss");

            migrationBuilder.DropTable(
                name: "ArticleFeedbackComments");

            migrationBuilder.DropTable(
                name: "ArticleFeedbacks");

            migrationBuilder.AddColumn<int>(
                name: "MaxNumberOfRefunds",
                table: "Settings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MinOrderAmount",
                table: "Settings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Tickets_ArticleFeedbackId",
                table: "Articles",
                column: "ArticleFeedbackId",
                principalTable: "Tickets",
                principalColumn: "Id");
        }
    }
}
