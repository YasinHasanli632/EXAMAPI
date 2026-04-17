using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamInfrastucture.Migrations
{
    /// <inheritdoc />
    public partial class CreateDatabseNewprop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentTasks_TeacherId",
                table: "StudentTasks");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckedAt",
                table: "StudentTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClassRoomId",
                table: "StudentTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Feedback",
                table: "StudentTasks",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StudentTasks",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionFileUrl",
                table: "StudentTasks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionLink",
                table: "StudentTasks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionText",
                table: "StudentTasks",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "StudentTasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaskGroupKey",
                table: "StudentTasks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTasks_ClassRoomId",
                table: "StudentTasks",
                column: "ClassRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTasks_TaskGroupKey",
                table: "StudentTasks",
                column: "TaskGroupKey");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTasks_TeacherId_ClassRoomId_TaskGroupKey",
                table: "StudentTasks",
                columns: new[] { "TeacherId", "ClassRoomId", "TaskGroupKey" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentTasks_ClassRooms_ClassRoomId",
                table: "StudentTasks",
                column: "ClassRoomId",
                principalTable: "ClassRooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentTasks_ClassRooms_ClassRoomId",
                table: "StudentTasks");

            migrationBuilder.DropIndex(
                name: "IX_StudentTasks_ClassRoomId",
                table: "StudentTasks");

            migrationBuilder.DropIndex(
                name: "IX_StudentTasks_TaskGroupKey",
                table: "StudentTasks");

            migrationBuilder.DropIndex(
                name: "IX_StudentTasks_TeacherId_ClassRoomId_TaskGroupKey",
                table: "StudentTasks");

            migrationBuilder.DropColumn(
                name: "CheckedAt",
                table: "StudentTasks");

            migrationBuilder.DropColumn(
                name: "ClassRoomId",
                table: "StudentTasks");

            migrationBuilder.DropColumn(
                name: "Feedback",
                table: "StudentTasks");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StudentTasks");

            migrationBuilder.DropColumn(
                name: "SubmissionFileUrl",
                table: "StudentTasks");

            migrationBuilder.DropColumn(
                name: "SubmissionLink",
                table: "StudentTasks");

            migrationBuilder.DropColumn(
                name: "SubmissionText",
                table: "StudentTasks");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "StudentTasks");

            migrationBuilder.DropColumn(
                name: "TaskGroupKey",
                table: "StudentTasks");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTasks_TeacherId",
                table: "StudentTasks",
                column: "TeacherId");
        }
    }
}
