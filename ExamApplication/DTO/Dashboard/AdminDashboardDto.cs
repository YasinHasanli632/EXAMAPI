using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Dashboard
{
    public class AdminDashboardDto
    {
        public int ActiveTeacherCount { get; set; }

        public int TodayAttendanceRate { get; set; }

        public int TotalClasses { get; set; }

        public int TotalTeachers { get; set; }

        public int TotalStudents { get; set; }

        public int ActiveExams { get; set; }

        public int TodayExamCount { get; set; }

        public int TotalSubjects { get; set; }

        public int TotalAdmins { get; set; }

        public List<AdminDashboardActivityDto> Activities { get; set; } = new();
    }

    public class AdminDashboardActivityDto
    {
        public int Id { get; set; }

        public string? Action { get; set; }

        public string? EntityName { get; set; }

        public int? EntityId { get; set; }

        public DateTime? ActionTime { get; set; }

        public string? PerformedBy { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }
    }
}
