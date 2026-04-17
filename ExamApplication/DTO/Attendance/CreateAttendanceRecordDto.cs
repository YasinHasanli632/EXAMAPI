using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    public class CreateAttendanceRecordDto
    {
        public int StudentId { get; set; }

        // Present / Absent / Late
        public string Status { get; set; } = "Present";

        public string? Notes { get; set; }

        // YENI
        public string? AbsenceReasonType { get; set; }

        // YENI
        public string? AbsenceReasonNote { get; set; }

        // YENI
        public TimeSpan? LateArrivalTime { get; set; }

        // YENI
        public string? LateNote { get; set; }
    }
}
