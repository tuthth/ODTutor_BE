using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class CertificateType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CertificateTypeId",
                table: "TutorCertificates",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CertificateTypes",
                columns: table => new
                {
                    CertificateTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTypes", x => x.CertificateTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TutorCertificates_CertificateTypeId",
                table: "TutorCertificates",
                column: "CertificateTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_TutorCertificates_CertificateTypes_CertificateTypeId",
                table: "TutorCertificates",
                column: "CertificateTypeId",
                principalTable: "CertificateTypes",
                principalColumn: "CertificateTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TutorCertificates_CertificateTypes_CertificateTypeId",
                table: "TutorCertificates");

            migrationBuilder.DropTable(
                name: "CertificateTypes");

            migrationBuilder.DropIndex(
                name: "IX_TutorCertificates_CertificateTypeId",
                table: "TutorCertificates");

            migrationBuilder.DropColumn(
                name: "CertificateTypeId",
                table: "TutorCertificates");
        }
    }
}
