using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Add_Some_Fields_In_Course : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "StudyTime",
                table: "Courses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TutorSubjectId",
                table: "Courses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TutorSubjectId",
                table: "Courses",
                column: "TutorSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_TutorSubjects_TutorSubjectId",
                table: "Courses",
                column: "TutorSubjectId",
                principalTable: "TutorSubjects",
                principalColumn: "TutorSubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_TutorSubjects_TutorSubjectId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TutorSubjectId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StudyTime",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "TutorSubjectId",
                table: "Courses");
        }
    }
}
