using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamInfrastucture.Migrations
{
    /// <inheritdoc />
    public partial class Createnewtableandupdateproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ExamAccessCodes_AccessCode",
                table: "ExamAccessCodes");

            migrationBuilder.AddColumn<int>(
                name: "FullScreenExitCount",
                table: "StudentExams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoSubmitted",
                table: "StudentExams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "StudentExams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastActivityAt",
                table: "StudentExams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "StudentExams",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "TabSwitchCount",
                table: "StudentExams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WarningCount",
                table: "StudentExams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSavedAt",
                table: "StudentAnswers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GeneratedAt",
                table: "ExamAccessCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UsedAt",
                table: "ExamAccessCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExamSecurityLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentExamId = table.Column<int>(type: "int", nullable: false),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSecurityLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamSecurityLogs_StudentExams_StudentExamId",
                        column: x => x.StudentExamId,
                        principalTable: "StudentExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamAccessCodes_AccessCode",
                table: "ExamAccessCodes",
                column: "AccessCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamSecurityLogs_StudentExamId",
                table: "ExamSecurityLogs",
                column: "StudentExamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamSecurityLogs");

            migrationBuilder.DropIndex(
                name: "IX_ExamAccessCodes_AccessCode",
                table: "ExamAccessCodes");

            migrationBuilder.DropColumn(
                name: "FullScreenExitCount",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "IsAutoSubmitted",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "LastActivityAt",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "TabSwitchCount",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "WarningCount",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "LastSavedAt",
                table: "StudentAnswers");

            migrationBuilder.DropColumn(
                name: "GeneratedAt",
                table: "ExamAccessCodes");

            migrationBuilder.DropColumn(
                name: "UsedAt",
                table: "ExamAccessCodes");

            migrationBuilder.CreateIndex(
                name: "IX_ExamAccessCodes_AccessCode",
                table: "ExamAccessCodes",
                column: "AccessCode");
        }
    }
}
