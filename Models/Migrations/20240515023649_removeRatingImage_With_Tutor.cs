using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class removeRatingImage_With_Tutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorRatingImages_Tutors_TutorId",
                table: "TutorRatingImages");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorRatingImages_Tutors_TutorId",
                table: "TutorRatingImages",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "TutorId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorRatingImages_Tutors_TutorId",
                table: "TutorRatingImages");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorRatingImages_Tutors_TutorId",
                table: "TutorRatingImages",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "TutorId");
        }
    }
}
