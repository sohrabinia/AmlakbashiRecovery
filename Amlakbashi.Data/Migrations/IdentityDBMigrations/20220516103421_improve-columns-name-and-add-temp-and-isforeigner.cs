using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations.IdentityDBMigrations
{
    public partial class improvecolumnsnameandaddtempandisforeigner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "AspNetUsers",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "SendVerification",
                table: "AspNetUsers",
                newName: "LastSentVerifyCodeDate");

            migrationBuilder.RenameColumn(
                name: "EmailCode",
                table: "AspNetUsers",
                newName: "EmailVerifyCode");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "AspNetUsers",
                newName: "VerifyCode");

            migrationBuilder.AddColumn<bool>(
                name: "IsForeigner",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Temp",
                table: "AspNetUsers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsForeigner",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Temp",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "VerifyCode",
                table: "AspNetUsers",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AspNetUsers",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "LastSentVerifyCodeDate",
                table: "AspNetUsers",
                newName: "SendVerification");

            migrationBuilder.RenameColumn(
                name: "EmailVerifyCode",
                table: "AspNetUsers",
                newName: "EmailCode");
        }
    }
}
