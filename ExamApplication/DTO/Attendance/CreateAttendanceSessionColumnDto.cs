using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class CreateAttendanceSessionColumnDto
    {
        public int ClassRoomId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }

        public DateTime SessionDate { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public string? Notes { get; set; }

        // YENI
        public AttendanceSessionType SessionType { get; set; } = AttendanceSessionType.RegularLesson;
    }
}
