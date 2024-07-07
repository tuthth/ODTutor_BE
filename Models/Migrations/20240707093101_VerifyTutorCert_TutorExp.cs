using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class VerifyTutorCert_TutorExp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "TutorExperiences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "TutorCertificates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "TutorExperiences");

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "TutorCertificates");
        }
    }
}
