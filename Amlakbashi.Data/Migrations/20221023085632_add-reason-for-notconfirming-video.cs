using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class addreasonfornotconfirmingvideo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReasonForNotConfirmingVideo",
                table: "Residences",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReasonForNotConfirmingVideo",
                table: "Residences");
        }
    }
}
