using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class addvideotoresidence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VideoId",
                table: "Residences",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "VideoStatus",
                table: "Residences",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "Files",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateIndex(
                name: "IX_Residences_VideoId",
                table: "Residences",
                column: "VideoId",
                unique: true,
                filter: "[VideoId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Residences_Files_VideoId",
                table: "Residences",
                column: "VideoId",
                principalTable: "Files",
                principalColumn: "FileID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Residences_Files_VideoId",
                table: "Residences");

            migrationBuilder.DropIndex(
                name: "IX_Residences_VideoId",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "VideoId",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "VideoStatus",
                table: "Residences");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Files");
        }
    }
}
