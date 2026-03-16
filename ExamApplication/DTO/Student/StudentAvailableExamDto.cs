using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student üçün siyahıda göstərilən exam məlumatlarını daşıyır.
    public class StudentAvailableExamDto
    {
        // Exam Id-si.
        public int ExamId { get; set; }

        // Exam başlığı.
        public string Title { get; set; } = null!;


        // Fənn adı.
        public string SubjectName { get; set; } = null!;

        // Exam başlama vaxtı.
        public DateTime StartDate { get; set; }

        // Exam bitmə vaxtı.
        public DateTime EndDate { get; set; }

        // Müddət dəqiqə ilə.
        public int DurationMinutes { get; set; }

        // Student bu exam-ə başlayıb ya yox.
        public bool HasStarted { get; set; }

        // Student bu exam-i bitirib ya yox.
        public bool IsCompleted { get; set; }

        // Əgər session yaranıbsa onun Id-si.
        public int? StudentExamId { get; set; }
    }
}
