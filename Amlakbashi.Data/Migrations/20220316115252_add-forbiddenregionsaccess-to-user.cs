using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class addforbiddenregionsaccesstouser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessType",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AdminLoginCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ForgetCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginPriority",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResponseFrom",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ResponseTo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SendVerification",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserGeneralType",
                table: "Users",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "ThirdPersonTell",
                table: "Users",
                newName: "ThirdPersonPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Tell",
                table: "Users",
                newName: "LandlinePhoneNumber");

            migrationBuilder.RenameColumn(
                name: "PrizeCredit",
                table: "Users",
                newName: "GiftWalletAmount");

            migrationBuilder.RenameColumn(
                name: "Mobile2",
                table: "Users",
                newName: "PhoneNumber3");

            migrationBuilder.RenameColumn(
                name: "Mobile",
                table: "Users",
                newName: "PhoneNumber2");

            migrationBuilder.RenameColumn(
                name: "MainMobile",
                table: "Users",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "LName",
                table: "Users",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "FName",
                table: "Users",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "Credit",
                table: "Users",
                newName: "WalletAmount");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Users",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Users",
                newName: "Id");

            migrationBuilder.AddColumn<bool>(
                name: "ForbiddenRegionsAccess",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForbiddenRegionsAccess",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "WalletAmount",
                table: "Users",
                newName: "Credit");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Users",
                newName: "UserGeneralType");

            migrationBuilder.RenameColumn(
                name: "ThirdPersonPhoneNumber",
                table: "Users",
                newName: "ThirdPersonTell");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber3",
                table: "Users",
                newName: "Mobile2");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber2",
                table: "Users",
                newName: "Mobile");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Users",
                newName: "MainMobile");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "LName");

            migrationBuilder.RenameColumn(
                name: "LandlinePhoneNumber",
                table: "Users",
                newName: "Tell");

            migrationBuilder.RenameColumn(
                name: "GiftWalletAmount",
                table: "Users",
                newName: "PrizeCredit");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Users",
                newName: "FName");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Users",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "UserID");

            migrationBuilder.AddColumn<int>(
                name: "AccessType",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AdminLoginCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForgetCode",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoginPriority",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResponseFrom",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResponseTo",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SendVerification",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
