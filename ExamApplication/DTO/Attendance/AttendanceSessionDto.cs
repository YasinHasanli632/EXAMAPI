using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    public class AttendanceSessionDto
    {
        public int Id { get; set; }

        public int ClassRoomId { get; set; }
        public string ClassName { get; set; } = string.Empty;

        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;

        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;

        public DateTime SessionDate { get; set; }

        // YENI
        public TimeSpan? StartTime { get; set; }

        // YENI
        public TimeSpan? EndTime { get; set; }

        public string? Notes { get; set; }

        // YENI
        public string SessionType { get; set; } = string.Empty;

        // YENI
        public bool IsExtraLesson { get; set; }

        // YENI
        public bool IsLocked { get; set; }

        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
    }
}
