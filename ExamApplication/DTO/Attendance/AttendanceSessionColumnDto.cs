using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class AttendanceSessionColumnDto
    {
        public int SessionId { get; set; }

        public DateTime SessionDate { get; set; }
        public string SessionDateText { get; set; } = string.Empty;

        public string? StartTimeText { get; set; }
        public string? EndTimeText { get; set; }

        // YENI
        public string SessionType { get; set; } = string.Empty;

        // YENI
        public bool IsExtraLesson { get; set; }

        // YENI
        public bool IsLocked { get; set; }

        public string? Notes { get; set; }
    }
}
