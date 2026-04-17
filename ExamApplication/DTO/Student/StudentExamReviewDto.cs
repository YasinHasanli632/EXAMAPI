using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentExamReviewDto
    {
        public int StudentExamId { get; set; }

        public int ExamId { get; set; }

        public string ExamTitle { get; set; } = null!;

        public string SubjectName { get; set; } = string.Empty;

        public string TeacherName { get; set; } = string.Empty;

        public DateTime ExamDate { get; set; }

        public decimal Score { get; set; }

        public List<StudentExamReviewQuestionDto> Questions { get; set; } = new();
    }
}
