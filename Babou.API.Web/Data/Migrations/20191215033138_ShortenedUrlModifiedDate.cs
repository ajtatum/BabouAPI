using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Babou.API.Web.Data.Migrations
{
    public partial class ShortenedUrlModifiedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedOn",
                table: "ShortenedUrls",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModifiedOn",
                table: "ShortenedUrls");
        }
    }
}
