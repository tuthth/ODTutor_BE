using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTutorCertificate_With_Subject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorCertificates_TutorSubjects_TutorSubjectId",
                table: "TutorCertificates");

            migrationBuilder.DropIndex(
                name: "IX_TutorCertificates_TutorSubjectId",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "TutorSubjectId",
                table: "TutorCertificates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "TutorCertificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "TutorCertificates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TutorCertificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TutorSubjectId",
                table: "TutorCertificates",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TutorCertificates_TutorSubjectId",
                table: "TutorCertificates",
                column: "TutorSubjectId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TutorCertificates_TutorSubjects_TutorSubjectId",
                table: "TutorCertificates",
                column: "TutorSubjectId",
                principalTable: "TutorSubjects",
                principalColumn: "TutorSubjectId");
        }
    }
}
