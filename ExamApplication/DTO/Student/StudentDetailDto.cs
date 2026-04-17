using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentDetailDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;

        // YENI
        public string FirstName { get; set; } = string.Empty;

        // YENI
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = null!;

        // YENI
        public string? PhoneNumber { get; set; }

        // YENI
        public string ParentName { get; set; } = string.Empty;

        // YENI
        public string? ParentPhone { get; set; }

        // YENI
        public string? Address { get; set; }

        // YENI
        public string Gender { get; set; } = "Unknown";

        public string StudentNumber { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string? ClassName { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public decimal AverageScore { get; set; }
        public int ExamsCount { get; set; }
        public double AttendanceRate { get; set; }
        public string? PhotoUrl { get; set; }

        // YENI
        public int TasksCount { get; set; }

        // YENI
        public int CompletedTasksCount { get; set; }

        // YENI
        public int AbsentCount { get; set; }

        // YENI
        public int LateCount { get; set; }

        public List<StudentExamSummaryDto> Exams { get; set; } = new();
        public List<StudentAttendanceSummaryDto> Attendance { get; set; } = new();
        public List<StudentTaskDto> Tasks { get; set; } = new();
    }
}
