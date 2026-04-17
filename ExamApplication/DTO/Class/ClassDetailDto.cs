using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassDetailDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string AcademicYear { get; set; } = string.Empty;

        public string Room { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Aktiv";

        public int MaxStudentCount { get; set; }

        public decimal AverageScore { get; set; }

        public double AttendanceRate { get; set; }

        public int ExamCount { get; set; }

        public string? CreatedAt { get; set; }

        public List<ClassStudentOptionDto> Students { get; set; } = new();

        public List<ClassTopStudentDto> TopStudents { get; set; } = new();

        public List<ClassSubjectOptionDto> Subjects { get; set; } = new();

        public List<ClassExamItemDto> Exams { get; set; } = new();

        public List<ClassTeacherSubjectRowDto> TeacherSubjectRows { get; set; } = new();
    }
}
