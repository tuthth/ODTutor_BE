using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class StudentRequest_and_PrivateCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTransaction_Courses_CourseTransactionId",
                table: "CourseTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTransaction_Wallets_ReceiverWalletId",
                table: "CourseTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTransaction_Wallets_SenderWalletId",
                table: "CourseTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseTransaction",
                table: "CourseTransaction");

            migrationBuilder.RenameTable(
                name: "CourseTransaction",
                newName: "CourseTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_CourseTransaction_SenderWalletId",
                table: "CourseTransactions",
                newName: "IX_CourseTransactions_SenderWalletId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseTransaction_ReceiverWalletId",
                table: "CourseTransactions",
                newName: "IX_CourseTransactions_ReceiverWalletId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseTransactions",
                table: "CourseTransactions",
                column: "CourseTransactionId");

            migrationBuilder.CreateTable(
                name: "PrivateCourses",
                columns: table => new
                {
                    PrivateCourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateCourses", x => x.PrivateCourseId);
                    table.ForeignKey(
                        name: "FK_PrivateCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId");
                    table.ForeignKey(
                        name: "FK_PrivateCourses_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId");
                });

            migrationBuilder.CreateTable(
                name: "StudentRequests",
                columns: table => new
                {
                    StudentRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentRequests", x => x.StudentRequestId);
                    table.ForeignKey(
                        name: "FK_StudentRequests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId");
                    table.ForeignKey(
                        name: "FK_StudentRequests_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "SubjectId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateCourses_CourseId",
                table: "PrivateCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateCourses_StudentId",
                table: "PrivateCourses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequests_StudentId",
                table: "StudentRequests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentRequests_SubjectId",
                table: "StudentRequests",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTransactions_Courses_CourseTransactionId",
                table: "CourseTransactions",
                column: "CourseTransactionId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTransactions_Wallets_ReceiverWalletId",
                table: "CourseTransactions",
                column: "ReceiverWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTransactions_Wallets_SenderWalletId",
                table: "CourseTransactions",
                column: "SenderWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseTransactions_Courses_CourseTransactionId",
                table: "CourseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTransactions_Wallets_ReceiverWalletId",
                table: "CourseTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseTransactions_Wallets_SenderWalletId",
                table: "CourseTransactions");

            migrationBuilder.DropTable(
                name: "PrivateCourses");

            migrationBuilder.DropTable(
                name: "StudentRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseTransactions",
                table: "CourseTransactions");

            migrationBuilder.RenameTable(
                name: "CourseTransactions",
                newName: "CourseTransaction");

            migrationBuilder.RenameIndex(
                name: "IX_CourseTransactions_SenderWalletId",
                table: "CourseTransaction",
                newName: "IX_CourseTransaction_SenderWalletId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseTransactions_ReceiverWalletId",
                table: "CourseTransaction",
                newName: "IX_CourseTransaction_ReceiverWalletId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseTransaction",
                table: "CourseTransaction",
                column: "CourseTransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTransaction_Courses_CourseTransactionId",
                table: "CourseTransaction",
                column: "CourseTransactionId",
                principalTable: "Courses",
                principalColumn: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTransaction_Wallets_ReceiverWalletId",
                table: "CourseTransaction",
                column: "ReceiverWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseTransaction_Wallets_SenderWalletId",
                table: "CourseTransaction",
                column: "SenderWalletId",
                principalTable: "Wallets",
                principalColumn: "WalletId");
        }
    }
}
