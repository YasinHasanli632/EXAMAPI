using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student exam session məlumatını daşıyır.
    public class StudentExamSessionDto
    {
        // StudentExam Id-si.
        public int StudentExamId { get; set; }

        // Student Id-si.
        public int StudentId { get; set; }

        // Exam Id-si.
        public int ExamId { get; set; }

        // Başlama vaxtı.
        public DateTime StartTime { get; set; }

        // Bitmə vaxtı.
        public DateTime? EndTime { get; set; }

        // Session tamamlanıb ya yox.
        public bool IsCompleted { get; set; }

        // Toplanmış bal.
        public decimal Score { get; set; }
    }
}
