using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Promotion_Tutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TutorId",
                table: "Promotions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_TutorId",
                table: "Promotions",
                column: "TutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Tutors_TutorId",
                table: "Promotions",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "TutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Tutors_TutorId",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_TutorId",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "TutorId",
                table: "Promotions");
        }
    }
}
