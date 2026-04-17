using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Attendance
{
    // YENI
    public class AttendanceBoardFilterDto
    {
        public int ClassRoomId { get; set; }
        public int SubjectId { get; set; }
        public int TeacherId { get; set; }

        // YENI
        public int Year { get; set; }
        public int Month { get; set; }
    }
}
