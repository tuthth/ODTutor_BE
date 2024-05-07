using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class SenderReceiverWalletId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTransactions_Wallets_WalletId",
                table: "BookingTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTransaction_Wallets_WalletId",
                table: "CourseTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_CourseTransaction_WalletId",
                table: "CourseTransaction");

            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "WalletTransactions",
                newName: "SenderWalletId");

            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "CourseTransaction",
                newName: "SenderWalletId");

            migrationBuilder.RenameColumn(
                name: "WalletId",
                table: "BookingTransactions",
                newName: "SenderWalletId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingTransactions_WalletId",
                table: "BookingTransactions",
                newName: "IX_BookingTransactions_SenderWalletId");

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiverWalletId",
                table: "WalletTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiverWalletId",
                table: "CourseTransaction",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ReceiverWalletId",
                table: "BookingTransactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_ReceiverWalletId",
                table: "WalletTransactions",
                column: "ReceiverWalletId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTransaction_ReceiverWalletId",
                table: "CourseTransaction",
                column: "ReceiverWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTransactions_Wallets_SenderWalletId",
                table: "BookingTransactions",
                column: "SenderWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTransaction_Wallets_ReceiverWalletId",
                table: "CourseTransaction",
                column: "ReceiverWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_ReceiverWalletId",
                table: "WalletTransactions",
                column: "ReceiverWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookingTransactions_Wallets_SenderWalletId",
                table: "BookingTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTransaction_Wallets_ReceiverWalletId",
                table: "CourseTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Wallets_ReceiverWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransactions_ReceiverWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropIndex(
                name: "IX_CourseTransaction_ReceiverWalletId",
                table: "CourseTransaction");

            migrationBuilder.DropColumn(
                name: "ReceiverWalletId",
                table: "WalletTransactions");

            migrationBuilder.DropColumn(
                name: "ReceiverWalletId",
                table: "CourseTransaction");

            migrationBuilder.DropColumn(
                name: "ReceiverWalletId",
                table: "BookingTransactions");

            migrationBuilder.RenameColumn(
                name: "SenderWalletId",
                table: "WalletTransactions",
                newName: "WalletId");

            migrationBuilder.RenameColumn(
                name: "SenderWalletId",
                table: "CourseTransaction",
                newName: "WalletId");

            migrationBuilder.RenameColumn(
                name: "SenderWalletId",
                table: "BookingTransactions",
                newName: "WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_BookingTransactions_SenderWalletId",
                table: "BookingTransactions",
                newName: "IX_BookingTransactions_WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTransaction_WalletId",
                table: "CourseTransaction",
                column: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookingTransactions_Wallets_WalletId",
                table: "BookingTransactions",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTransaction_Wallets_WalletId",
                table: "CourseTransaction",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Wallets_WalletId",
                table: "WalletTransactions",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");
        }
    }
}
