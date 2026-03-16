using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student-in konkret exam nəticə ekranı üçün məlumatları daşıyır.
    public class StudentExamResultDto
    {
        // StudentExam Id-si.
        public int StudentExamId { get; set; }

        // Exam Id-si.
        public int ExamId { get; set; }

        // Exam adı.
        public string ExamTitle { get; set; } = null!;

        // Fənn adı.
        public string SubjectName { get; set; } = null!;

        // Student tam adı.
        public string StudentFullName { get; set; } = null!;

        // Student nömrəsi.
        public string StudentNumber { get; set; } = null!;

        // Başlama vaxtı.
        public DateTime StartTime { get; set; }

        // Bitmə vaxtı.
        public DateTime? EndTime { get; set; }

        // Toplam bal.
        public decimal Score { get; set; }

        // Tamamlanma statusu.
        public bool IsCompleted { get; set; }

        // Ümumi sual sayı.
        public int TotalQuestions { get; set; }

        // Cavab verilmiş sual sayı.
        public int AnsweredQuestions { get; set; }

        // Cavablar.
        public List<StudentAnswerDto> Answers { get; set; } = new();
    }
}
