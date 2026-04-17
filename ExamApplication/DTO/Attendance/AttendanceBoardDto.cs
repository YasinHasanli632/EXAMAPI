using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class AttendanceBoardDto
    {
        public int ClassRoomId { get; set; }
        public string ClassName { get; set; } = string.Empty;

        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;

        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;

        public int Year { get; set; }
        public int Month { get; set; }

        public int TotalStudents { get; set; }
        public int TotalSessions { get; set; }
        public double AttendanceRate { get; set; }

        public List<AttendanceSessionColumnDto> Sessions { get; set; } = new();
        public List<AttendanceBoardStudentRowDto> Students { get; set; } = new();
    }
}
