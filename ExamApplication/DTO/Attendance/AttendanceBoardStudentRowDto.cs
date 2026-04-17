using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class AttendanceBoardStudentRowDto
    {
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public string StudentPhotoUrl { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;

        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }

        public double AttendanceRate { get; set; }

        public List<AttendanceBoardCellDto> Cells { get; set; } = new();
    }
}
