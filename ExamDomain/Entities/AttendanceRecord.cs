using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class AttendanceRecord : AuditableEntity
    {
        public int AttendanceSessionId { get; set; }
        public AttendanceSession AttendanceSession { get; set; } = null!;

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;

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
