using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class AttendanceChangeRequestDto
    {
        public int Id { get; set; }

        public int ClassRoomId { get; set; }
        public string ClassName { get; set; } = string.Empty;

        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;

        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;

        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;

        public DateTime AttendanceDate { get; set; }

        public string CurrentStatus { get; set; } = string.Empty;
        public string RequestedStatus { get; set; } = string.Empty;

        public string RequestedChangeReason { get; set; } = string.Empty;

        public string? RequestedAbsenceReasonType { get; set; }
        public string? RequestedAbsenceReasonNote { get; set; }
        public string? RequestedLateArrivalTime { get; set; }
        public string? RequestedLateNote { get; set; }

        public int RequestedByTeacherId { get; set; }
        public DateTime RequestedAt { get; set; }

        public string RequestStatus { get; set; } = string.Empty;
        public int? ReviewedByAdminId { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? ReviewNote { get; set; }
    }
}
