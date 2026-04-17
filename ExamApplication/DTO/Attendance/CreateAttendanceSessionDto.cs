using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    public class CreateAttendanceSessionDto
    {
        public int ClassRoomId { get; set; }

        public int SubjectId { get; set; }

        public int TeacherId { get; set; }

        public DateTime SessionDate { get; set; }

        public string? Notes { get; set; }

        // YENI
        public TimeSpan? StartTime { get; set; }

        // YENI
        public TimeSpan? EndTime { get; set; }

        // YENI
        public AttendanceSessionType SessionType { get; set; } = AttendanceSessionType.RegularLesson;

        public List<CreateAttendanceRecordDto> Records { get; set; } = new();
    }
}
