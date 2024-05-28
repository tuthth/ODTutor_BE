using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class removew_two_attribute : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorExperiences_Tutors_TutorId",
                table: "TutorExperiences");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "TutorCertificates");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorExperiences_Tutors_TutorId",
                table: "TutorExperiences",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "TutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorExperiences_Tutors_TutorId",
                table: "TutorExperiences");

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId",
                table: "TutorCertificates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_TutorExperiences_Tutors_TutorId",
                table: "TutorExperiences",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "TutorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
    