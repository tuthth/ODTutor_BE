using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseSlot_FixStudentCourse_Schedule_Course : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "StudentCourses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "StudentCourses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseSlots",
                columns: table => new
                {
                    CourseSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SlotNumber = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSlots", x => x.CourseSlotId);
                    table.ForeignKey(
                        name: "FK_CourseSlots_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId");
                });

            migrationBuilder.CreateTable(
                name: "CourseSchedules",
                columns: table => new
                {
                    CourseSlotId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSchedules", x => new { x.CourseSlotId, x.ScheduleId });
                    table.ForeignKey(
                        name: "FK_CourseSchedules_CourseSlots_CourseSlotId",
                        column: x => x.CourseSlotId,
                        principalTable: "CourseSlots",
                        principalColumn: "CourseSlotId");
                    table.ForeignKey(
                        name: "FK_CourseSchedules_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "ScheduleId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseSchedules_ScheduleId",
                table: "CourseSchedules",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseSlots_CourseId",
                table: "CourseSlots",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseSchedules");

            migrationBuilder.DropTable(
                name: "CourseSlots");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StudentCourses");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "StudentCourses");
        }
    }
}
