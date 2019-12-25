using Microsoft.EntityFrameworkCore.Migrations;

namespace Babou.API.Web.Data.Migrations
{
    public partial class ApplicationServices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationServices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserServices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationServiceId = table.Column<int>(nullable: false),
                    ApplicationUserId = table.Column<string>(nullable: false),
                    ApplicationSettings = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUserServices_ApplicationServices_ApplicationServiceId",
                        column: x => x.ApplicationServiceId,
                        principalTable: "ApplicationServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserServices_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserServices_ApplicationServiceId",
                table: "ApplicationUserServices",
                column: "ApplicationServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserServices_ApplicationUserId",
                table: "ApplicationUserServices",
                column: "ApplicationUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserServices");

            migrationBuilder.DropTable(
                name: "ApplicationServices");
        }
    }
}
