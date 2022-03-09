using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class addlicensetoadvertise : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_ModifiedWalletTransactionId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "BuildingDirection",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "ImageThumbGenerateStatus",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "PrepaymentPrice",
                table: "Advertises");

            migrationBuilder.AddColumn<bool>(
                name: "License",
                table: "Advertises",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "LicenseFileId",
                table: "Advertises",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "Advertises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ModifiedWalletTransactionId",
                table: "WalletTransactions",
                column: "ModifiedWalletTransactionId",
                unique: true,
                filter: "[ModifiedWalletTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Advertises_LicenseFileId",
                table: "Advertises",
                column: "LicenseFileId",
                unique: true,
                filter: "[LicenseFileId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Advertises_Files_LicenseFileId",
                table: "Advertises",
                column: "LicenseFileId",
                principalTable: "Files",
                principalColumn: "FileID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advertises_Files_LicenseFileId",
                table: "Advertises");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_ModifiedWalletTransactionId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Advertises_LicenseFileId",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "License",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "LicenseFileId",
                table: "Advertises");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "Advertises");

            migrationBuilder.AddColumn<int>(
                name: "BuildingDirection",
                table: "Advertises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ImageThumbGenerateStatus",
                table: "Advertises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrepaymentPrice",
                table: "Advertises",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ModifiedWalletTransactionId",
                table: "WalletTransactions",
                column: "ModifiedWalletTransactionId");
        }
    }
}
