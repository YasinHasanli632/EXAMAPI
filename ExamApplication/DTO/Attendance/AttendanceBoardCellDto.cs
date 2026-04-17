using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class AttendanceBoardCellDto
    {
        public int SessionId { get; set; }
        public int? AttendanceRecordId { get; set; }

        public string? Status { get; set; }
        public string? Notes { get; set; }

        public string? AbsenceReasonType { get; set; }
        public string? AbsenceReasonNote { get; set; }

        public string? LateArrivalTime { get; set; }
        public string? LateNote { get; set; }

        public bool HasRecord { get; set; }
        public bool IsEditable { get; set; }
        public bool IsLocked { get; set; }
    }
}
