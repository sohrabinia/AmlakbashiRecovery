using Microsoft.EntityFrameworkCore.Migrations;

namespace Amlakbashi.Data.Migrations
{
    public partial class updatepaymentsentities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_CreditTransactions_Reserves_ReserveID",
            //    table: "CreditTransactions");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_CreditTransactions_Users_UserID",
            //    table: "CreditTransactions");

            //migrationBuilder.DropPrimaryKey(
            //    name: "PK_CreditTransactions",
            //    table: "CreditTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.CreditTransactions_dbo.Reserves_ReserveID",
                table: "CreditTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_dbo.CreditTransactions_dbo.Users_UserID",
                table: "CreditTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_dbo.CreditTransactions",
                table: "CreditTransactions");

            migrationBuilder.DropColumn(
                name: "AdvertiseContactID",
                table: "CreditTransactions");

            migrationBuilder.RenameTable(
                name: "CreditTransactions",
                newName: "WalletTransactions");

            migrationBuilder.RenameColumn(
                name: "ReservePaymentID",
                table: "ReservePayments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Payments",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "RefID",
                table: "Payments",
                newName: "ReferenceNumber");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Payments",
                newName: "CreateDate");

            migrationBuilder.RenameColumn(
                name: "BankId",
                table: "Payments",
                newName: "Bank");

            migrationBuilder.RenameColumn(
                name: "Authority",
                table: "Payments",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "PaymentID",
                table: "Payments",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "TransactionCauseString",
                table: "WalletTransactions",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "TransactionCause",
                table: "WalletTransactions",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "RemainedPrice",
                table: "WalletTransactions",
                newName: "WalletRemainingAmount");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "WalletTransactions",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "CreditTransactionID",
                table: "WalletTransactions",
                newName: "Id");

            //migrationBuilder.RenameIndex(
            //    name: "IX_CreditTransactions_UserID",
            //    table: "WalletTransactions",
            //    newName: "IX_WalletTransactions_UserID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_CreditTransactions_ReserveID",
            //    table: "WalletTransactions",
            //    newName: "IX_WalletTransactions_ReserveID");

            migrationBuilder.RenameIndex(
                name: "IX_UserID",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_ReserveID",
                table: "WalletTransactions",
                newName: "IX_WalletTransactions_ReserveID");

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "ReservePayments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Method",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "ReservePaymentId",
                table: "Payments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TraceNumber",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "WalletTransactionId",
                table: "Payments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModifiedWalletTransactionId",
                table: "WalletTransactions",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId",
                table: "WalletTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "WalletTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletTransactions",
                table: "WalletTransactions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReservePayments_PaymentId",
                table: "ReservePayments",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReservePaymentId",
                table: "Payments",
                column: "ReservePaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_WalletTransactionId",
                table: "Payments",
                column: "WalletTransactionId",
                unique: true,
                filter: "[WalletTransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ModifiedWalletTransactionId",
                table: "WalletTransactions",
                column: "ModifiedWalletTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_PaymentId",
                table: "WalletTransactions",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_ReservePayments_ReservePaymentId",
                table: "Payments",
                column: "ReservePaymentId",
                principalTable: "ReservePayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_WalletTransactions_WalletTransactionId",
                table: "Payments",
                column: "WalletTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservePayments_Payments_PaymentId",
                table: "ReservePayments",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Payments_PaymentId",
                table: "WalletTransactions",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Reserves_ReserveID",
                table: "WalletTransactions",
                column: "ReserveID",
                principalTable: "Reserves",
                principalColumn: "ReserveID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Users_UserID",
                table: "WalletTransactions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_WalletTransactions_ModifiedWalletTransactionId",
                table: "WalletTransactions",
                column: "ModifiedWalletTransactionId",
                principalTable: "WalletTransactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_ReservePayments_ReservePaymentId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_WalletTransactions_WalletTransactionId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservePayments_Payments_PaymentId",
                table: "ReservePayments");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Payments_PaymentId",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Reserves_ReserveID",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Users_UserID",
                table: "WalletTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_WalletTransactions_ModifiedWalletTransactionId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ReservePayments_PaymentId",
                table: "ReservePayments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_ReservePaymentId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_WalletTransactionId",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletTransactions",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_ModifiedWalletTransactionId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_PaymentId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "ReservePayments");

            migrationBuilder.DropColumn(
                name: "Method",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ReservePaymentId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TraceNumber",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "WalletTransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ModifiedWalletTransactionId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "WalletTransactions");

            migrationBuilder.RenameTable(
                name: "WalletTransactions",
                newName: "CreditTransactions");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ReservePayments",
                newName: "ReservePaymentID");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Payments",
                newName: "Authority");

            migrationBuilder.RenameColumn(
                name: "ReferenceNumber",
                table: "Payments",
                newName: "RefID");

            migrationBuilder.RenameColumn(
                name: "CreateDate",
                table: "Payments",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "Bank",
                table: "Payments",
                newName: "BankId");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Payments",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Payments",
                newName: "PaymentID");

            migrationBuilder.RenameColumn(
                name: "WalletRemainingAmount",
                table: "CreditTransactions",
                newName: "RemainedPrice");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "CreditTransactions",
                newName: "TransactionCause");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CreditTransactions",
                newName: "TransactionCauseString");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "CreditTransactions",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CreditTransactions",
                newName: "CreditTransactionID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_WalletTransactions_UserID",
            //    table: "CreditTransactions",
            //    newName: "IX_CreditTransactions_UserID");

            //migrationBuilder.RenameIndex(
            //    name: "IX_WalletTransactions_ReserveID",
            //    table: "CreditTransactions",
            //    newName: "IX_CreditTransactions_ReserveID");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_UserID",
                table: "CreditTransactions",
                newName: "IX_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_WalletTransactions_ReserveID",
                table: "CreditTransactions",
                newName: "IX_ReserveID");

            migrationBuilder.AddColumn<long>(
                name: "AdvertiseContactID",
                table: "CreditTransactions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            //migrationBuilder.AddPrimaryKey(
            //    name: "PK_CreditTransactions",
            //    table: "CreditTransactions",
            //    column: "CreditTransactionID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_CreditTransactions_Reserves_ReserveID",
            //    table: "CreditTransactions",
            //    column: "ReserveID",
            //    principalTable: "Reserves",
            //    principalColumn: "ReserveID",
            //    onDelete: ReferentialAction.Restrict);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_CreditTransactions_Users_UserID",
            //    table: "CreditTransactions",
            //    column: "UserID",
            //    principalTable: "Users",
            //    principalColumn: "UserID",
            //    onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddPrimaryKey(
                name: "PK_dbo.CreditTransactions",
                table: "CreditTransactions",
                column: "CreditTransactionID");

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.CreditTransactions_dbo.Reserves_ReserveID",
                table: "CreditTransactions",
                column: "ReserveID",
                principalTable: "Reserves",
                principalColumn: "ReserveID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_dbo.CreditTransactions_dbo.Users_UserID",
                table: "CreditTransactions",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
