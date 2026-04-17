using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamInfrastucture.Migrations
{
    /// <inheritdoc />
    public partial class Newproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AbsenceReasonType",
                table: "AttendanceRecords",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LateArrivalTime",
                table: "AttendanceRecords",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LateTime",
                table: "AttendanceRecords",
                type: "time",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttendanceChangeRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendanceSessionId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CurrentStatus = table.Column<int>(type: "int", nullable: false),
                    RequestedStatus = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LateTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    RequestedByTeacherId = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    ApprovedByAdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceChangeRequests_AttendanceSessions_AttendanceSessionId",
                        column: x => x.AttendanceSessionId,
                        principalTable: "AttendanceSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceChangeRequests_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceChangeRequests_Teachers_RequestedByTeacherId",
                        column: x => x.RequestedByTeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChangeRequests_AttendanceSessionId",
                table: "AttendanceChangeRequests",
                column: "AttendanceSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChangeRequests_RequestedAt",
                table: "AttendanceChangeRequests",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChangeRequests_RequestedByTeacherId",
                table: "AttendanceChangeRequests",
                column: "RequestedByTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChangeRequests_StudentId",
                table: "AttendanceChangeRequests",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "AbsenceReasonType",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "LateArrivalTime",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "LateTime",
                table: "AttendanceRecords");
        }
    }
}
