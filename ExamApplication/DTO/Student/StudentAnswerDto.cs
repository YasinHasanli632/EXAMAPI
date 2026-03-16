using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // Student-in verdiyi cavab məlumatını daşıyır.
    public class StudentAnswerDto
    {
        // StudentAnswer Id-si.
        public int StudentAnswerId { get; set; }

        // Aid olduğu student exam session Id-si.
        public int StudentExamId { get; set; }

        // Aid olduğu exam question Id-si.
        public int ExamQuestionId { get; set; }

        // Test sual üçün seçilən variant Id-si.
        public int? SelectedOptionId { get; set; }

        // Açıq sual üçün yazılan mətn.
        public string? AnswerText { get; set; }

        // Bu cavaba verilmiş bal.
        public decimal PointsAwarded { get; set; }
    }
}
