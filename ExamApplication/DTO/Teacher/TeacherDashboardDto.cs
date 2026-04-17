using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class TeacherDashboardDto
    {
        public int TeacherId { get; set; }

        public string FullName { get; set; } = null!;

        public int TotalClasses { get; set; }

        public int TotalStudents { get; set; }

        public int TotalExams { get; set; }

        public int TotalTasks { get; set; }

        public int PendingTasks { get; set; }

        public int UnreadNotificationsCount { get; set; }

        public List<TeacherMyClassRoomListItemDto> Classes { get; set; } = new();
    }
}
