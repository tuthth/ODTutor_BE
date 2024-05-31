using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Add_DBSet_TutorAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorAction_Moderators_ModeratorId",
                table: "TutorAction");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorAction_Tutors_TutorId",
                table: "TutorAction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TutorAction",
                table: "TutorAction");

            migrationBuilder.RenameTable(
                name: "TutorAction",
                newName: "TutorActions");

            migrationBuilder.RenameIndex(
                name: "IX_TutorAction_TutorId",
                table: "TutorActions",
                newName: "IX_TutorActions_TutorId");

            migrationBuilder.RenameIndex(
                name: "IX_TutorAction_ModeratorId",
                table: "TutorActions",
                newName: "IX_TutorActions_ModeratorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TutorActions",
                table: "TutorActions",
                column: "TutorActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorActions_Moderators_ModeratorId",
                table: "TutorActions",
                column: "ModeratorId",
                principalTable: "Moderators",
                principalColumn: "ModeratorId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorActions_Tutors_TutorId",
                table: "TutorActions",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "TutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorActions_Moderators_ModeratorId",
                table: "TutorActions");

            migrationBuilder.DropForeignKey(
                name: "FK_TutorActions_Tutors_TutorId",
                table: "TutorActions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TutorActions",
                table: "TutorActions");

            migrationBuilder.RenameTable(
                name: "TutorActions",
                newName: "TutorAction");

            migrationBuilder.RenameIndex(
                name: "IX_TutorActions_TutorId",
                table: "TutorAction",
                newName: "IX_TutorAction_TutorId");

            migrationBuilder.RenameIndex(
                name: "IX_TutorActions_ModeratorId",
                table: "TutorAction",
                newName: "IX_TutorAction_ModeratorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TutorAction",
                table: "TutorAction",
                column: "TutorActionId");

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
    }
}
