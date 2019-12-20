using Microsoft.EntityFrameworkCore.Migrations;

namespace AJT.API.Web.Data.Migrations
{
    public partial class AddShortUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortUrl",
                table: "ShortenedUrls",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortUrl",
                table: "ShortenedUrls");
        }
    }
}
