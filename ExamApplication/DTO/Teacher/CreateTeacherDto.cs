using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher
{
    public class CreateTeacherDto
    {
        // Müəllimə bağlı user hesabının Id-si
        public int UserId { get; set; }

        // Müəllimin tam adı
        public string FullName { get; set; } = null!;

        // Müəllimin bölməsi
        public string Department { get; set; } = null!;

        // YENI
        public string? Specialization { get; set; }

        // YENI
        public TeacherStatus Status { get; set; } = TeacherStatus.Active;
    }
}
