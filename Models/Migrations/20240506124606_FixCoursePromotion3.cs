using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class FixCoursePromotion3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoursePromotionPromotions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursePromotions",
                table: "CoursePromotions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursePromotions",
                table: "CoursePromotions",
                columns: new[] { "PromotionId", "CourseId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePromotions_Promotions_PromotionId",
                table: "CoursePromotions",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "PromotionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoursePromotions_Promotions_PromotionId",
                table: "CoursePromotions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoursePromotions",
                table: "CoursePromotions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoursePromotions",
                table: "CoursePromotions",
                column: "PromotionId");

            migrationBuilder.CreateTable(
                name: "CoursePromotionPromotions",
                columns: table => new
                {
                    CoursePromotionsNavigationsPromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromotionsNavigationPromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePromotionPromotions", x => new { x.CoursePromotionsNavigationsPromotionId, x.PromotionsNavigationPromotionId });
                    table.ForeignKey(
                        name: "FK_CoursePromotionPromotions_CoursePromotions_CoursePromotionsNavigationsPromotionId",
                        column: x => x.CoursePromotionsNavigationsPromotionId,
                        principalTable: "CoursePromotions",
                        principalColumn: "PromotionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursePromotionPromotions_Promotions_PromotionsNavigationPromotionId",
                        column: x => x.PromotionsNavigationPromotionId,
                        principalTable: "Promotions",
                        principalColumn: "PromotionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursePromotionPromotions_PromotionsNavigationPromotionId",
                table: "CoursePromotionPromotions",
                column: "PromotionsNavigationPromotionId");
        }
    }
}
