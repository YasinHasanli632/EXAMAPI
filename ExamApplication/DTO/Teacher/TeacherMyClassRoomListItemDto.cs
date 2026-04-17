using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class TeacherMyClassRoomListItemDto
    {
        public int ClassRoomId { get; set; }

        public string ClassRoomName { get; set; } = null!;

        public int StudentCount { get; set; }

        public int ExamCount { get; set; }

        public decimal AverageScore { get; set; }

        public double AttendanceRate { get; set; }

        public List<string> SubjectNames { get; set; } = new();

        // YENI
        public string? TopStudentName { get; set; }

        // YENI
        public decimal? TopStudentScore { get; set; }
    }
}
