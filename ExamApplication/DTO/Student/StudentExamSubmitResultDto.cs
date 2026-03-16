using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Exam submit nəticəsini daşıyır.
    public class StudentExamSubmitResultDto
    {
        // StudentExam Id-si.
        public int StudentExamId { get; set; }

        // Exam Id-si.
        public int ExamId { get; set; }

        // Başlama vaxtı.
        public DateTime StartTime { get; set; }

        // Bitmə vaxtı.
        public DateTime EndTime { get; set; }

        // Exam tamamlandı statusu.
        public bool IsCompleted { get; set; }

        // Toplam score.
        public decimal Score { get; set; }

        // İstifadəçiyə göstəriləcək mesaj.
        public string Message { get; set; } = null!;
    }
}
