using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class updatereservepropertiesandaddearlycheckout : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Reserves_Advertises_AdvertiseID",
            //    table: "Reserves");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Reserves_Users_HostUserID",
            //    table: "Reserves");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Reserves_Users_UserID",
            //    table: "Reserves");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Reserves_dbo.Advertises_AdvertiseID",
                table: "Reserves");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Reserves_dbo.Users_HostUserID",
                table: "Reserves");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.Reserves_dbo.Users_UserID",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "ExcludeGroupPayment",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "GuestCallDate",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "HostCallDate",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "SupportState",
                table: "Reserves");

            migrationBuilder.DropColumn(
                name: "SupporterIds",
                table: "Reserves");

            migrationBuilder.RenameColumn(
                name: "PrizeTransactionID",
                table: "Reserves",
                newName: "PrizeTransactionId");

            migrationBuilder.RenameColumn(
                name: "CouponID",
                table: "Reserves",
                newName: "CouponId");

            migrationBuilder.RenameColumn(
                name: "shouldFollow",
                table: "Reserves",
                newName: "ShouldFollowUp");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Reserves",
                newName: "GuestId");

            migrationBuilder.RenameColumn(
                name: "HostUserID",
                table: "Reserves",
                newName: "HostId");

            migrationBuilder.RenameColumn(
                name: "AdvertiseID",
                table: "Reserves",
                newName: "ResidenceId");

            migrationBuilder.RenameColumn(
                name: "AccVisitedByGuest",
                table: "Reserves",
                newName: "VisitResidenceByGuest");

            migrationBuilder.RenameColumn(
                name: "ReserveID",
                table: "Reserves",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "PaymentHasError",
                table: "Reserves",
                newName: "EarlyCheckout");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Reserves_UserID",
            //    table: "Reserves",
            //    newName: "IX_Reserves_GuestId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Reserves_HostUserID",
            //    table: "Reserves",
            //    newName: "IX_Reserves_HostId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Reserves_AdvertiseID",
            //    table: "Reserves",
            //    newName: "IX_Reserves_ResidenceId");

            migrationBuilder.RenameIndex(
                name: "IX_UserID",
                table: "Reserves",
                newName: "IX_Reserves_GuestId");

            migrationBuilder.RenameIndex(
                name: "IX_HostUserID",
                table: "Reserves",
                newName: "IX_Reserves_HostId");

            migrationBuilder.RenameIndex(
                name: "IX_AdvertiseID",
                table: "Reserves",
                newName: "IX_Reserves_ResidenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Advertises_ResidenceId",
                table: "Reserves",
                column: "ResidenceId",
                principalTable: "Advertises",
                principalColumn: "AdvertiseID",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Users_GuestId",
                table: "Reserves",
                column: "GuestId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Users_HostId",
                table: "Reserves",
                column: "HostId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_Advertises_ResidenceId",
                table: "Reserves");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_Users_GuestId",
                table: "Reserves");

            migrationBuilder.DropForeignKey(
                name: "FK_Reserves_Users_HostId",
                table: "Reserves");

            migrationBuilder.RenameColumn(
                name: "PrizeTransactionId",
                table: "Reserves",
                newName: "PrizeTransactionID");

            migrationBuilder.RenameColumn(
                name: "CouponId",
                table: "Reserves",
                newName: "CouponID");

            migrationBuilder.RenameColumn(
                name: "VisitResidenceByGuest",
                table: "Reserves",
                newName: "AccVisitedByGuest");

            migrationBuilder.RenameColumn(
                name: "ShouldFollowUp",
                table: "Reserves",
                newName: "shouldFollow");

            migrationBuilder.RenameColumn(
                name: "ResidenceId",
                table: "Reserves",
                newName: "AdvertiseID");

            migrationBuilder.RenameColumn(
                name: "HostId",
                table: "Reserves",
                newName: "HostUserID");

            migrationBuilder.RenameColumn(
                name: "GuestId",
                table: "Reserves",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Reserves",
                newName: "ReserveID");

            migrationBuilder.RenameColumn(
                name: "EarlyCheckout",
                table: "Reserves",
                newName: "PaymentHasError");

            migrationBuilder.RenameIndex(
                name: "IX_Reserves_ResidenceId",
                table: "Reserves",
                newName: "IX_Reserves_AdvertiseID");

            migrationBuilder.RenameIndex(
                name: "IX_Reserves_HostId",
                table: "Reserves",
                newName: "IX_Reserves_HostUserID");

            migrationBuilder.RenameIndex(
                name: "IX_Reserves_GuestId",
                table: "Reserves",
                newName: "IX_Reserves_UserID");

            migrationBuilder.AddColumn<bool>(
                name: "ExcludeGroupPayment",
                table: "Reserves",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "GuestCallDate",
                table: "Reserves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "HostCallDate",
                table: "Reserves",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Reserves",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SupportState",
                table: "Reserves",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SupporterIds",
                table: "Reserves",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Advertises_AdvertiseID",
                table: "Reserves",
                column: "AdvertiseID",
                principalTable: "Advertises",
                principalColumn: "AdvertiseID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Users_HostUserID",
                table: "Reserves",
                column: "HostUserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reserves_Users_UserID",
                table: "Reserves",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
