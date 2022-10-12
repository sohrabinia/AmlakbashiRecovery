using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class addnewamenitiestoresidence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Balcony",
                table: "Residences",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EuropeanToiletType",
                table: "Residences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Filming",
                table: "Residences",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VillaType",
                table: "Residences",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balcony",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "EuropeanToiletType",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "Filming",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "VillaType",
                table: "Residences");
        }
    }
}
