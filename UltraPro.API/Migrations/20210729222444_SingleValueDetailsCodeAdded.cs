using Microsoft.EntityFrameworkCore.Migrations;

namespace UltraPro.API.Migrations
{
    public partial class SingleValueDetailsCodeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "SingleValueDetails",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "SingleValueDetails");
        }
    }
}
