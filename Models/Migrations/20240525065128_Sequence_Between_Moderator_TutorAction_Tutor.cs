using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Sequence_Between_Moderator_TutorAction_Tutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityCardBack",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "IdentityCardFront",
                table: "Tutors");

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetingLink",
                table: "TutorAction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TutorAction_ModeratorId",
                table: "TutorAction",
                column: "ModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_TutorAction_TutorId",
                table: "TutorAction",
                column: "TutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorAction_Moderators_ModeratorId",
                table: "TutorAction",
                column: "ModeratorId",
                principalTable: "Moderators",
                principalColumn: "ModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorAction_Tutors_TutorId",
                table: "TutorAction",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "TutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorAction_Moderators_ModeratorId",
                table: "TutorAction");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorAction_Tutors_TutorId",
                table: "TutorAction");

            migrationBuilder.DropIndex(
                name: "IX_TutorAction_ModeratorId",
                table: "TutorAction");

            migrationBuilder.DropIndex(
                name: "IX_TutorAction_TutorId",
                table: "TutorAction");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "MeetingLink",
                table: "TutorAction");

            migrationBuilder.AddColumn<string>(
                name: "IdentityCardBack",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IdentityCardFront",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
