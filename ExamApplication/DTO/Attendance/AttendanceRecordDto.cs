using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    public class AttendanceRecordDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentPhotoUrl { get; set; } = string.Empty;

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
