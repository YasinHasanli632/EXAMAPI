using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student üçün exam detail ekranını daşıyır.
    public class StudentExamDetailDto
    {
        // Exam Id-si.
        public int ExamId { get; set; }

        // Exam başlığı.
        public string Title { get; set; } = null!;

        // Exam açıqlaması.

        // Fənn adı.
        public string SubjectName { get; set; } = null!;

        // Exam başlama vaxtı.
        public DateTime StartDate { get; set; }

        // Exam bitmə vaxtı.
        public DateTime EndDate { get; set; }

        // Müddət dəqiqə ilə.
        public int DurationMinutes { get; set; }

        // Toplam sual sayı.
        public int TotalQuestionCount { get; set; }

        // Toplam bal.
        public decimal TotalPoints { get; set; }

        // Student başlamışdır ya yox.
        public bool HasStarted { get; set; }

        // Student exam-i tamamlayıb ya yox.
        public bool IsCompleted { get; set; }

        // Əgər session varsa onun Id-si.
        public int? StudentExamId { get; set; }

        // Studentə görünən suallar.
        public List<StudentExamQuestionDto> Questions { get; set; } = new();
    }
}
