using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentExamReviewOptionDto
    {
        public int Id { get; set; }

        public string Text { get; set; } = null!;

        public bool IsCorrect { get; set; }
    }
}
