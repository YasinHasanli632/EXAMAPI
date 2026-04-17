using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Dashboard
{
    public class TeacherDashboardDto
    {
        public int TeacherId { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;

        public int TeacherStatus { get; set; }

        public int TotalClasses { get; set; }

        public int TotalStudents { get; set; }

        public int TotalExams { get; set; }

        public int TotalTasks { get; set; }

        public int PendingTasks { get; set; }

        public int CompletedTasks { get; set; }

        public int TotalSubjects { get; set; }

        public int UnreadNotificationsCount { get; set; }

        public List<TeacherDashboardClassItemDto> Classes { get; set; } = new();

        public List<TeacherDashboardTaskItemDto> RecentTasks { get; set; } = new();
    }

    public class TeacherDashboardClassItemDto
    {
        public int ClassRoomId { get; set; }

        public string ClassRoomName { get; set; } = string.Empty;

        public int StudentCount { get; set; }

        public int ExamCount { get; set; }

        public List<string> SubjectNames { get; set; } = new();
    }

    public class TeacherDashboardTaskItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}
