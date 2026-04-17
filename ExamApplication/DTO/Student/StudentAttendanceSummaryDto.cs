using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentAttendanceSummaryDto
    {
        public int AttendanceSessionId { get; set; }

        public DateTime SessionDate { get; set; }

        public string SubjectName { get; set; } = null!;

        public string TeacherName { get; set; } = null!;

        public string Status { get; set; } = null!;
        // YENI
        public TimeSpan? StartTime { get; set; }

        // YENI
        public TimeSpan? EndTime { get; set; }

        // YENI
        public string? Note
        {
            get; set;
        }
    }
}
