using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class CreateAttendanceChangeRequestDto
    {
        public int ClassRoomId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }
        public int StudentId { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string RequestedStatus { get; set; } = string.Empty;

        // YENI
        public string RequestedChangeReason { get; set; } = string.Empty;

        // YENI
        public string? RequestedAbsenceReasonType { get; set; }

        // YENI
        public string? RequestedAbsenceReasonNote { get; set; }

        // YENI
        public string? RequestedLateArrivalTime { get; set; }

        // YENI
        public string? RequestedLateNote { get; set; }

        public int RequestedByTeacherId { get; set; }
    }
}
