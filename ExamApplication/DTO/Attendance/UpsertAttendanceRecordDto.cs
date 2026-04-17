using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class UpsertAttendanceRecordDto
    {
        public int StudentId { get; set; }

        // Present / Absent / Late
        public string Status { get; set; } = "Present";

        public string? Notes { get; set; }

        public string? AbsenceReasonType { get; set; }
        public string? AbsenceReasonNote { get; set; }

        public TimeSpan? LateArrivalTime { get; set; }
        public string? LateNote { get; set; }
    }
}
