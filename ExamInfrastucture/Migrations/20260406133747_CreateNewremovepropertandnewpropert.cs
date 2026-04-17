using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamInfrastucture.Migrations
{
    /// <inheritdoc />
    public partial class CreateNewremovepropertandnewpropert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceChangeRequests_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_ClassRoomId",
                table: "AttendanceSessions");

            migrationBuilder.DropColumn(
                name: "LateTime",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "AttendanceChangeRequests");

            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "AttendanceChangeRequests",
                newName: "RequestedLateNote");

            migrationBuilder.RenameColumn(
                name: "LateTime",
                table: "AttendanceChangeRequests",
                newName: "RequestedLateArrivalTime");

            migrationBuilder.RenameColumn(
                name: "AttendanceSessionId",
                table: "AttendanceChangeRequests",
                newName: "TeacherId");

            migrationBuilder.RenameColumn(
                name: "ApprovedByAdminId",
                table: "AttendanceChangeRequests",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceChangeRequests_AttendanceSessionId",
                table: "AttendanceChangeRequests",
                newName: "IX_AttendanceChangeRequests_TeacherId");

            migrationBuilder.AddColumn<string>(
                name: "AbsenceReasonNote",
                table: "AttendanceRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LateNote",
                table: "AttendanceRecords",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendanceDate",
                table: "AttendanceChangeRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ClassRoomId",
                table: "AttendanceChangeRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AttendanceChangeRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "AttendanceChangeRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestStatus",
                table: "AttendanceChangeRequests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestedAbsenceReasonNote",
                table: "AttendanceChangeRequests",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedAbsenceReasonType",
                table: "AttendanceChangeRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestedChangeReason",
                table: "AttendanceChangeRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReviewNote",
                table: "AttendanceChangeRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "AttendanceChangeRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByAdminId",
                table: "AttendanceChangeRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "AttendanceChangeRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AttendanceChangeRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_ClassRoomId_SubjectId_TeacherId_SessionDate",
                table: "AttendanceSessions",
                columns: new[] { "ClassRoomId", "SubjectId", "TeacherId", "SessionDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChangeRequests_ClassRoomId",
                table: "AttendanceChangeRequests",
                column: "ClassRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChangeRequests_ClassRoomId_SubjectId_StudentId_AttendanceDate_RequestStatus",
                table: "AttendanceChangeRequests",
                columns: new[] { "ClassRoomId", "SubjectId", "StudentId", "AttendanceDate", "RequestStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceChangeRequests_SubjectId",
                table: "AttendanceChangeRequests",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceChangeRequests_ClassRooms_ClassRoomId",
                table: "AttendanceChangeRequests",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceChangeRequests_Subjects_SubjectId",
                table: "AttendanceChangeRequests",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceChangeRequests_Teachers_TeacherId",
                table: "AttendanceChangeRequests",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceChangeRequests_ClassRooms_ClassRoomId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceChangeRequests_Subjects_SubjectId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceChangeRequests_Teachers_TeacherId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceSessions_ClassRoomId_SubjectId_TeacherId_SessionDate",
                table: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceChangeRequests_ClassRoomId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceChangeRequests_ClassRoomId_SubjectId_StudentId_AttendanceDate_RequestStatus",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceChangeRequests_SubjectId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "AbsenceReasonNote",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "LateNote",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "AttendanceDate",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "ClassRoomId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "RequestStatus",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "RequestedAbsenceReasonNote",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "RequestedAbsenceReasonType",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "RequestedChangeReason",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "ReviewNote",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "ReviewedByAdminId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "AttendanceChangeRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AttendanceChangeRequests");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "AttendanceChangeRequests",
                newName: "ApprovedByAdminId");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "AttendanceChangeRequests",
                newName: "AttendanceSessionId");

            migrationBuilder.RenameColumn(
                name: "RequestedLateNote",
                table: "AttendanceChangeRequests",
                newName: "Reason");

            migrationBuilder.RenameColumn(
                name: "RequestedLateArrivalTime",
                table: "AttendanceChangeRequests",
                newName: "LateTime");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceChangeRequests_TeacherId",
                table: "AttendanceChangeRequests",
                newName: "IX_AttendanceChangeRequests_AttendanceSessionId");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LateTime",
                table: "AttendanceRecords",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "AttendanceChangeRequests",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "AttendanceChangeRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_ClassRoomId",
                table: "AttendanceSessions",
                column: "ClassRoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceChangeRequests_AttendanceSessions_AttendanceSessionId",
                table: "AttendanceChangeRequests",
                column: "AttendanceSessionId",
                principalTable: "AttendanceSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
