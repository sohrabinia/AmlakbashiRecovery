using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class modifyinstantreserve : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelInstantReserveLimit",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InstantReserveAccess",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "InstantReserveCancels",
                table: "Advertises");

            migrationBuilder.RenameColumn(
                name: "AdvertiseMode",
                table: "Advertises",
                newName: "Mode");

            migrationBuilder.RenameColumn(
                name: "AdvertiseID",
                table: "Advertises",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "DisableInstantReserve",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "InstantReserveDate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResidenceId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "Date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstantReserveDate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InstantReserveDate_Advertises_ResidenceId",
                        column: x => x.ResidenceId,
                        principalTable: "Advertises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InstantReserveDate_ResidenceId",
                table: "InstantReserveDate",
                column: "ResidenceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InstantReserveDate");

            migrationBuilder.DropColumn(
                name: "DisableInstantReserve",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Mode",
                table: "Advertises",
                newName: "AdvertiseMode");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Advertises",
                newName: "AdvertiseID");

            migrationBuilder.AddColumn<int>(
                name: "CancelInstantReserveLimit",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InstantReserveAccess",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InstantReserveCancels",
                table: "Advertises",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
