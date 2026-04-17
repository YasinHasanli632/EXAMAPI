using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamInfrastucture.Migrations
{
    /// <inheritdoc />
    public partial class Createnewproperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AutoGradedScore",
                table: "StudentExams",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsReviewed",
                table: "StudentExams",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ManualGradedScore",
                table: "StudentExams",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "StudentExams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedByTeacherId",
                table: "StudentExams",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "StudentExams",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "StudentAnswers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoGradedScore",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "IsReviewed",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "ManualGradedScore",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "ReviewedByTeacherId",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "StudentExams");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "StudentAnswers");
        }
    }
}
