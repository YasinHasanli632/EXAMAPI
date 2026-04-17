using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class GradeStudentExamResultDto
    {
        public int StudentExamId { get; set; }
        public bool IsReviewed { get; set; }
        public decimal AutoGradedScore { get; set; }
        public decimal ManualGradedScore { get; set; }
        public decimal TotalScore { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
