using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // YENI - SAGIRD UCUN
    public class StudentAttendanceItemDto
    {
        public int AttendanceSessionId { get; set; }

        public DateTime SessionDate { get; set; }

        public string ClassName { get; set; } = string.Empty;

        public string SubjectName { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? Notes { get; set; }

        // YENI - SAGIRD UCUN
        public string? AbsenceReasonType { get; set; }

        // YENI - SAGIRD UCUN
        public string? AbsenceReasonNote { get; set; }

        // YENI - SAGIRD UCUN
        public string? LateArrivalTime { get; set; }

        // YENI - SAGIRD UCUN
        public string? LateNote { get; set; }
    }
}
