using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Dashboard
{
    public class StudentDashboardDto
    {
        public int StudentId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string StudentNumber { get; set; } = string.Empty;

        public int StudentStatus { get; set; }

        public int? ClassRoomId { get; set; }

        public string? ClassRoomName { get; set; }

        public int MySubjectsCount { get; set; }

        public int UpcomingExamsCount { get; set; }

        public int CompletedExamsCount { get; set; }

        public decimal AverageScore { get; set; }

        public int UnreadNotificationsCount { get; set; }

        public List<StudentDashboardExamItemDto> Exams { get; set; } = new();

        public List<StudentDashboardTaskItemDto> RecentTasks { get; set; } = new();
    }

    public class StudentDashboardExamItemDto
    {
        public int ExamId { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }

    public class StudentDashboardTaskItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public DateTime? DueDate { get; set; }

        public int Status { get; set; }
    }
}
