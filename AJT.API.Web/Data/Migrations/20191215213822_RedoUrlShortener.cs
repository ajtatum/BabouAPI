using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Babou.API.Web.Data.Migrations
{
    public partial class RedoUrlShortener : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortenedUrls_AspNetUsers_CreatedBy",
                table: "ShortenedUrls");

            migrationBuilder.DropTable(
                name: "ShortenedUrlClicks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShortenedUrls",
                table: "ShortenedUrls");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_CreatedBy",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ShortenedUrls");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ShortenedUrls",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "ShortenedUrls",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "ShortenedUrls",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_ApplicationUserId",
                table: "ShortenedUrls",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShortenedUrls_AspNetUsers_ApplicationUserId",
                table: "ShortenedUrls",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShortenedUrls_AspNetUsers_ApplicationUserId",
                table: "ShortenedUrls");

            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_ApplicationUserId",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "Domain",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "ShortenedUrls");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ShortenedUrls",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShortenedUrls",
                table: "ShortenedUrls",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ShortenedUrlClicks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClickDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Referrer = table.Column<string>(type: "varchar(500)", nullable: true),
                    ShortenedUrlId = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortenedUrlClicks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortenedUrlClicks_ShortenedUrls_ShortenedUrlId",
                        column: x => x.ShortenedUrlId,
                        principalTable: "ShortenedUrls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_CreatedBy",
                table: "ShortenedUrls",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrlClicks_ShortenedUrlId",
                table: "ShortenedUrlClicks",
                column: "ShortenedUrlId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShortenedUrls_AspNetUsers_CreatedBy",
                table: "ShortenedUrls",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
