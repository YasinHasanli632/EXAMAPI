using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class AttendanceSession : AuditableEntity
    {
        public int ClassRoomId { get; set; }
        public ClassRoom ClassRoom { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = null!;

        public DateTime SessionDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string? Notes { get; set; }

        // YENI
        public AttendanceSessionType SessionType { get; set; } = AttendanceSessionType.RegularLesson;

        // YENI
        public bool IsLocked { get; set; } = false;

        public ICollection<AttendanceRecord> Records { get; set; } = new List<AttendanceRecord>();
    }
}
