using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

namespace Babou.API.Web.Data.Migrations
{
    public partial class ClickLocationInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "ShortenedUrlClicks",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "ShortenedUrlClicks",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<Point>(
                name: "Geography",
                table: "ShortenedUrlClicks",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "ShortenedUrlClicks",
                type: "varchar(50)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "ShortenedUrlClicks");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "ShortenedUrlClicks");

            migrationBuilder.DropColumn(
                name: "Geography",
                table: "ShortenedUrlClicks");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ShortenedUrlClicks");
        }
    }
}
