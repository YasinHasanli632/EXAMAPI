using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamInfrastucture.Migrations
{
    /// <inheritdoc />
    public partial class CreateNewtableandnewpropertys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentClasses_StudentId_ClassRoomId",
                table: "StudentClasses");

            migrationBuilder.DropIndex(
                name: "IX_ClassTeacherSubjects_ClassRoomId_SubjectId_TeacherId",
                table: "ClassTeacherSubjects");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Students",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StudentClasses",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "StudentClasses",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "LeftAt",
                table: "StudentClasses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "StudentAnswers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "StudentAnswers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClosedQuestionCount",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "ClosedQuestionScore",
                table: "Exams",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instructions",
                table: "Exams",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OpenQuestionCount",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuestionCount",
                table: "Exams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ExamQuestions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "ExamQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SelectionMode",
                table: "ExamQuestions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionKey",
                table: "ExamOptions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber",
                table: "ExamOptions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ClassTeacherSubjects",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "AcademicYear",
                table: "ClassRooms",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxStudentCount",
                table: "ClassRooms",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.CreateTable(
                name: "AttendanceSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassRoomId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    TeacherId = table.Column<int>(type: "int", nullable: false),
                    SessionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceSessions_ClassRooms_ClassRoomId",
                        column: x => x.ClassRoomId,
                        principalTable: "ClassRooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceSessions_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttendanceSessions_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswerOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentAnswerId = table.Column<int>(type: "int", nullable: false),
                    ExamOptionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswerOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswerOptions_ExamOptions_ExamOptionId",
                        column: x => x.ExamOptionId,
                        principalTable: "ExamOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswerOptions_StudentAnswers_StudentAnswerId",
                        column: x => x.StudentAnswerId,
                        principalTable: "StudentAnswers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AttendanceSessionId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_AttendanceSessions_AttendanceSessionId",
                        column: x => x.AttendanceSessionId,
                        principalTable: "AttendanceSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_StudentId_ClassRoomId",
                table: "StudentClasses",
                columns: new[] { "StudentId", "ClassRoomId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTeacherSubjects_ClassRoomId_SubjectId_TeacherId",
                table: "ClassTeacherSubjects",
                columns: new[] { "ClassRoomId", "SubjectId", "TeacherId" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_AttendanceSessionId_StudentId",
                table: "AttendanceRecords",
                columns: new[] { "AttendanceSessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_StudentId",
                table: "AttendanceRecords",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_ClassRoomId",
                table: "AttendanceSessions",
                column: "ClassRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_SubjectId",
                table: "AttendanceSessions",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_TeacherId",
                table: "AttendanceSessions",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswerOptions_ExamOptionId",
                table: "StudentAnswerOptions",
                column: "ExamOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswerOptions_StudentAnswerId_ExamOptionId",
                table: "StudentAnswerOptions",
                columns: new[] { "StudentAnswerId", "ExamOptionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "StudentAnswerOptions");

            migrationBuilder.DropTable(
                name: "AttendanceSessions");

            migrationBuilder.DropIndex(
                name: "IX_StudentClasses_StudentId_ClassRoomId",
                table: "StudentClasses");

            migrationBuilder.DropIndex(
                name: "IX_ClassTeacherSubjects_ClassRoomId_SubjectId_TeacherId",
                table: "ClassTeacherSubjects");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "LeftAt",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "ClosedQuestionCount",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ClosedQuestionScore",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Instructions",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "OpenQuestionCount",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "TotalQuestionCount",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "SelectionMode",
                table: "ExamQuestions");

            migrationBuilder.DropColumn(
                name: "OptionKey",
                table: "ExamOptions");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "ExamOptions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ClassTeacherSubjects");

            migrationBuilder.DropColumn(
                name: "AcademicYear",
                table: "ClassRooms");

            migrationBuilder.DropColumn(
                name: "MaxStudentCount",
                table: "ClassRooms");

            migrationBuilder.CreateIndex(
                name: "IX_StudentClasses_StudentId_ClassRoomId",
                table: "StudentClasses",
                columns: new[] { "StudentId", "ClassRoomId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassTeacherSubjects_ClassRoomId_SubjectId_TeacherId",
                table: "ClassTeacherSubjects",
                columns: new[] { "ClassRoomId", "SubjectId", "TeacherId" },
                unique: true);
        }
    }
}
