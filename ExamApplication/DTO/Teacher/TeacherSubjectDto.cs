using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class TeacherSubjectDto
    {
        // Əlaqənin Id-si
        public int Id { get; set; }

        // Fənnin Id-si
        public int SubjectId { get; set; }

        // Fənnin adı
        public string SubjectName { get; set; } = null!;

        // YENI
        public string? SubjectCode { get; set; }

        // YENI
        public bool IsActive { get; set; }
    }
}
