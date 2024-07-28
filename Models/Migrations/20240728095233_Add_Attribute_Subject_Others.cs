using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Add_Attribute_Subject_Others : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TutorSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "BookingContent",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TutorSubjectId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            // Cập nhật các bản ghi hiện có với giá trị hợp lệ
            migrationBuilder.Sql("UPDATE Bookings SET TutorSubjectId = (SELECT TOP 1 TutorSubjectId FROM TutorSubjects)");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TutorSubjectId",
                table: "Bookings",
                column: "TutorSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_TutorSubjects_TutorSubjectId",
                table: "Bookings",
                column: "TutorSubjectId",
                principalTable: "TutorSubjects",
                principalColumn: "TutorSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_TutorSubjects_TutorSubjectId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_TutorSubjectId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TutorSubjects");

            migrationBuilder.DropColumn(
                name: "BookingContent",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TutorSubjectId",
                table: "Bookings");
        }
    }
}
