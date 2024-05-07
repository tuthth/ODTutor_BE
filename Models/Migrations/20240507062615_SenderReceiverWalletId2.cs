using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class SenderReceiverWalletId2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_SenderWalletId",
                table: "WalletTransactions",
                column: "SenderWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTransaction_SenderWalletId",
                table: "CourseTransaction",
                column: "SenderWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTransactions_ReceiverWalletId",
                table: "BookingTransactions",
                column: "ReceiverWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTransactions_Wallets_ReceiverWalletId",
                table: "BookingTransactions",
                column: "ReceiverWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTransaction_Wallets_SenderWalletId",
                table: "CourseTransaction",
                column: "SenderWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_SenderWalletId",
                table: "WalletTransactions",
                column: "SenderWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTransactions_Wallets_ReceiverWalletId",
                table: "BookingTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTransaction_Wallets_SenderWalletId",
                table: "CourseTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_SenderWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_SenderWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_CourseTransaction_SenderWalletId",
                table: "CourseTransaction");

            migrationBuilder.DropIndex(
                name: "IX_BookingTransactions_ReceiverWalletId",
                table: "BookingTransactions");
        }
    }
}
