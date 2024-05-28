using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Create_Link_Between_Tutor_TutorExperience : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TutorExperience",
                table: "TutorExperience");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "TutorExperience");

            migrationBuilder.RenameTable(
                name: "TutorExperience",
                newName: "TutorExperiences");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndYear",
                table: "TutorExperiences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "TutorExperiences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TutorExperiences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TutorExperiences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TutorExperiences",
                table: "TutorExperiences",
                column: "TutorExperienceId");

            migrationBuilder.CreateIndex(
                name: "IX_TutorExperiences_TutorId",
                table: "TutorExperiences",
                column: "TutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorExperiences_Tutors_TutorId",
                table: "TutorExperiences",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "TutorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorExperiences_Tutors_TutorId",
                table: "TutorExperiences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TutorExperiences",
                table: "TutorExperiences");

            migrationBuilder.DropIndex(
                name: "IX_TutorExperiences_TutorId",
                table: "TutorExperiences");

            migrationBuilder.DropColumn(
                name: "EndYear",
                table: "TutorExperiences");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "TutorExperiences");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TutorExperiences");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TutorExperiences");

            migrationBuilder.RenameTable(
                name: "TutorExperiences",
                newName: "TutorExperience");

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "TutorExperience",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_TutorExperience",
                table: "TutorExperience",
                column: "TutorExperienceId");
        }
    }
}
