using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class NoLimitOTPs_4_User_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the unique index if it exists
            migrationBuilder.DropIndex(
                name: "IX_UserAuthentications_UserId",
                table: "UserAuthentications");

            // Add a non-unique index on UserId
            migrationBuilder.CreateIndex(
                name: "IX_UserAuthentications_UserId",
                table: "UserAuthentications",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Recreate the unique index if needed in the down method
            migrationBuilder.DropIndex(
                name: "IX_UserAuthentications_UserId",
                table: "UserAuthentications");

            migrationBuilder.CreateIndex(
                name: "IX_UserAuthentications_UserId",
                table: "UserAuthentications",
                column: "UserId",
                unique: true);
        }
    }
}
