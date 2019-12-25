using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Babou.API.Web.Data.Migrations
{
    public partial class ShortenedUrlsMisc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortenedUrls",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(50)", nullable: false),
                    LongUrl = table.Column<string>(type: "varchar(500)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortenedUrls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShortenedUrls_AspNetUsers_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShortenedUrlClicks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortenedUrlId = table.Column<string>(type: "varchar(50)", nullable: false),
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
                name: "IX_ShortenedUrlClicks_ShortenedUrlId",
                table: "ShortenedUrlClicks",
                column: "ShortenedUrlId");

            migrationBuilder.CreateIndex(
                name: "IX_ShortenedUrls_CreatedBy",
                table: "ShortenedUrls",
                column: "CreatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortenedUrlClicks");

            migrationBuilder.DropTable(
                name: "ShortenedUrls");
        }
    }
}
