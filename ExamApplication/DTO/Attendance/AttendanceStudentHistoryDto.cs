using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    public class AttendanceStudentHistoryDto
    {
        public int AttendanceSessionId { get; set; }
        public DateTime SessionDate { get; set; }

        public string ClassName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // YENI
        public string? AbsenceReasonType { get; set; }

        // YENI
        public string? AbsenceReasonNote { get; set; }

        // YENI
        public string? LateArrivalTime { get; set; }

        // YENI
        public string? LateNote { get; set; }
    }
}

