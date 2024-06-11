using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Add_New_Attribute_Expericent_Tutor_Certificate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TutorExperiences");

            migrationBuilder.DropColumn(
                name: "CertificateType",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "TutorCertificates");

            migrationBuilder.AddColumn<string>(
                name: "AttractiveTitle",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EducationExperience",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Motivation",
                table: "Tutors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EndYear",
                table: "TutorExperiences",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "tartDate",
                table: "TutorExperiences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CertificateDescription",
                table: "TutorCertificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CertificateFrom",
                table: "TutorCertificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CertificateName",
                table: "TutorCertificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EndYear",
                table: "TutorCertificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartYear",
                table: "TutorCertificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttractiveTitle",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "EducationExperience",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "Motivation",
                table: "Tutors");

            migrationBuilder.DropColumn(
                name: "tartDate",
                table: "TutorExperiences");

            migrationBuilder.DropColumn(
                name: "CertificateDescription",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "CertificateFrom",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "CertificateName",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "EndYear",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "StartYear",
                table: "TutorCertificates");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndYear",
                table: "TutorExperiences",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TutorExperiences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CertificateType",
                table: "TutorCertificates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "TutorCertificates",
                type: "datetime2",
                nullable: true);
        }
    }
}
