using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Babou.API.Web.Data.Migrations
{
    public partial class AddUrlShortenerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ShortenedUrls",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShortenedUrls",
                table: "ShortenedUrls",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ShortenedUrlClicks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortenedUrlId = table.Column<int>(nullable: false),
                    ClickDate = table.Column<DateTime>(nullable: false),
                    Referrer = table.Column<string>(type: "varchar(500)", nullable: true)
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
                name: "IX_ShortenedUrls_Token_Domain",
                table: "ShortenedUrls",
                columns: new[] { "Token", "Domain" },
                unique: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_ShortenedUrls_Token_Domain",
                table: "ShortenedUrls");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ShortenedUrls");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "ShortenedUrls",
                type: "nvarchar(450)",
                nullable: true);

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
    }
}
