using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student-in keçmiş exam sessiyaları üçün qısa məlumat daşıyır.
    public class StudentExamHistoryDto
    {
        // StudentExam Id-si.
        public int StudentExamId { get; set; }

        // Exam Id-si.
        public int ExamId { get; set; }

        // Exam başlığı.
        public string ExamTitle { get; set; } = null!;

        // Fənn adı.
        public string SubjectName { get; set; } = null!;

        // Başlama vaxtı.
        public DateTime StartTime { get; set; }

        // Bitmə vaxtı.
        public DateTime? EndTime { get; set; }

        // Tamamlanma statusu.
        public bool IsCompleted { get; set; }

        // Toplanan bal.
        public decimal Score { get; set; }
    }
}
