using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class Remove_OldSchedule_Replace_For_Three_Table_For_Tutor_Schedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TutorSchedules");

            migrationBuilder.CreateTable(
                name: "TutorWeekAvailables",
                columns: table => new
                {
                    TutorWeekAvailableId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TutorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorWeekAvailables", x => x.TutorWeekAvailableId);
                    table.ForeignKey(
                        name: "FK_TutorWeekAvailables_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutors",
                        principalColumn: "TutorId");
                });

            migrationBuilder.CreateTable(
                name: "TutorDateAvailables",
                columns: table => new
                {
                    TutorDateAvailableID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TutorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TutorWeekAvailableID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorDateAvailables", x => x.TutorDateAvailableID);
                    table.ForeignKey(
                        name: "FK_TutorDateAvailables_TutorWeekAvailables_TutorWeekAvailableID",
                        column: x => x.TutorWeekAvailableID,
                        principalTable: "TutorWeekAvailables",
                        principalColumn: "TutorWeekAvailableId");
                    table.ForeignKey(
                        name: "FK_TutorDateAvailables_Tutors_TutorID",
                        column: x => x.TutorID,
                        principalTable: "Tutors",
                        principalColumn: "TutorId");
                });

            migrationBuilder.CreateTable(
                name: "TutorSlotAvailables",
                columns: table => new
                {
                    TutorSlotAvailableID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TutorDateAvailableID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TutorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsBooked = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorSlotAvailables", x => x.TutorSlotAvailableID);
                    table.ForeignKey(
                        name: "FK_TutorSlotAvailables_TutorDateAvailables_TutorDateAvailableID",
                        column: x => x.TutorDateAvailableID,
                        principalTable: "TutorDateAvailables",
                        principalColumn: "TutorDateAvailableID");
                    table.ForeignKey(
                        name: "FK_TutorSlotAvailables_Tutors_TutorID",
                        column: x => x.TutorID,
                        principalTable: "Tutors",
                        principalColumn: "TutorId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TutorDateAvailables_TutorID",
                table: "TutorDateAvailables",
                column: "TutorID");

            migrationBuilder.CreateIndex(
                name: "IX_TutorDateAvailables_TutorWeekAvailableID",
                table: "TutorDateAvailables",
                column: "TutorWeekAvailableID");

            migrationBuilder.CreateIndex(
                name: "IX_TutorSlotAvailables_TutorDateAvailableID",
                table: "TutorSlotAvailables",
                column: "TutorDateAvailableID");

            migrationBuilder.CreateIndex(
                name: "IX_TutorSlotAvailables_TutorID",
                table: "TutorSlotAvailables",
                column: "TutorID");

            migrationBuilder.CreateIndex(
                name: "IX_TutorWeekAvailables_TutorId",
                table: "TutorWeekAvailables",
                column: "TutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TutorSlotAvailables");

            migrationBuilder.DropTable(
                name: "TutorDateAvailables");

            migrationBuilder.DropTable(
                name: "TutorWeekAvailables");

            migrationBuilder.CreateTable(
                name: "TutorSchedules",
                columns: table => new
                {
                    TutorScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TutorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActualEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsBooked = table.Column<bool>(type: "bit", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TutorSchedules", x => x.TutorScheduleId);
                    table.ForeignKey(
                        name: "FK_TutorSchedules_Tutors_TutorId",
                        column: x => x.TutorId,
                        principalTable: "Tutors",
                        principalColumn: "TutorId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TutorSchedules_TutorId",
                table: "TutorSchedules",
                column: "TutorId");
        }
    }
}
