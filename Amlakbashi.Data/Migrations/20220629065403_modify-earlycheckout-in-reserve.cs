using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class modifyearlycheckoutinreserve : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Reserves_Advertises_ResidenceId",
            //    table: "Reserves");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Reserves_Users_GuestId",
            //    table: "Reserves");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Reserves_Users_HostId",
            //    table: "Reserves");

            migrationBuilder.DropColumn(
                name: "EarlyCheckout",
                table: "Reserves");

            migrationBuilder.AddColumn<int>(
                name: "EarlyCheckoutStatus",
                table: "Reserves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Reserves_Advertises_ResidenceId",
            //    table: "Reserves",
            //    column: "ResidenceId",
            //    principalTable: "Advertises",
            //    principalColumn: "AdvertiseID",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Reserves_Users_GuestId",
            //    table: "Reserves",
            //    column: "GuestId",
            //    principalTable: "Users",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Reserves_Users_HostId",
            //    table: "Reserves",
            //    column: "HostId",
            //    principalTable: "Users",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Reserves_Advertises_ResidenceId",
            //    table: "Reserves");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Reserves_Users_GuestId",
            //    table: "Reserves");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Reserves_Users_HostId",
            //    table: "Reserves");

            migrationBuilder.DropColumn(
                name: "EarlyCheckoutStatus",
                table: "Reserves");

            migrationBuilder.AddColumn<bool>(
                name: "EarlyCheckout",
                table: "Reserves",
                type: "bit",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Reserves_Advertises_ResidenceId",
            //    table: "Reserves",
            //    column: "ResidenceId",
            //    principalTable: "Advertises",
            //    principalColumn: "AdvertiseID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Reserves_Users_GuestId",
            //    table: "Reserves",
            //    column: "GuestId",
            //    principalTable: "Users",
            //    principalColumn: "Id");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Reserves_Users_HostId",
            //    table: "Reserves",
            //    column: "HostId",
            //    principalTable: "Users",
            //    principalColumn: "Id");
        }
    }
}
