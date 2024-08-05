using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Some_Fields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fcm",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RequestRefreshTime",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "TotalSlots",
                table: "Courses",
                newName: "TotalStudent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalStudent",
                table: "Courses",
                newName: "TotalSlots");

            migrationBuilder.AddColumn<string>(
                name: "Fcm",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestRefreshTime",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }
    }
}
