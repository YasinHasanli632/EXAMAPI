using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamInfrastucture.Migrations
{
    /// <inheritdoc />
    public partial class CreatenewentitySystemsettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SchoolName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AcademicYear = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DefaultLanguage = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DefaultExamDurationMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 90),
                    AccessCodeActivationMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    LateEntryToleranceMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 10),
                    DefaultPassScore = table.Column<int>(type: "int", nullable: false, defaultValue: 51),
                    AutoSubmitOnEndTime = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AllowEarlySubmit = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AutoPublishResults = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ShowScoreImmediately = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowCorrectAnswersAfterCompletion = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ExamPublishNotificationEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ExamRescheduleNotificationEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ExamStartNotificationEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ResultNotificationEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AttendanceNotificationEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    TaskNotificationEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    AutoSaveIntervalSeconds = table.Column<int>(type: "int", nullable: false, defaultValue: 30),
                    SessionTimeoutMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 20),
                    MaxAccessCodeAttempts = table.Column<int>(type: "int", nullable: false, defaultValue: 5),
                    AllowReEntry = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemSettings");
        }
    }
}
