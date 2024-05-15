using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTutorImage_Tutor_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorRatingImages_Tutors_TutorId",
                table: "TutorRatingImages");

            migrationBuilder.DropIndex(
                name: "IX_TutorRatingImages_TutorId",
                table: "TutorRatingImages");

            migrationBuilder.DropColumn(
                name: "TutorId",
                table: "TutorRatingImages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TutorId",
                table: "TutorRatingImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TutorRatingImages_TutorId",
                table: "TutorRatingImages",
                column: "TutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorRatingImages_Tutors_TutorId",
                table: "TutorRatingImages",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "TutorId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
