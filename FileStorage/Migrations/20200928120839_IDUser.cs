using Microsoft.EntityFrameworkCore.Migrations;

namespace FileStorage.Migrations
{
    public partial class IDUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IDUser",
                table: "Movie",
                type: "nvarchar(450)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDUser",
                table: "Movie");
        }
    }
}
