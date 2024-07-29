using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Add_Some_Package_For_Tutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasBoughtSubscription",
                table: "Tutors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubcriptionEndDate",
                table: "Tutors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubcriptionStartDate",
                table: "Tutors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubcriptionType",
                table: "Tutors",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBoughtSubscription",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "SubcriptionEndDate",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "SubcriptionStartDate",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "SubcriptionType",
                table: "Tutors");
        }
    }
}
