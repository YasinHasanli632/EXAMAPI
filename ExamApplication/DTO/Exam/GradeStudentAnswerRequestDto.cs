using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class GradeStudentAnswerRequestDto
    {
        public int StudentAnswerId { get; set; }
        public decimal Score { get; set; }
        public string? TeacherFeedback { get; set; }
    }
}
