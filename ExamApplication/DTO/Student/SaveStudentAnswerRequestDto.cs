using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class SaveStudentAnswerRequestDto
    {
        public int StudentExamId { get; set; }
        public int ExamQuestionId { get; set; }

        // single choice üçün
        public int? SelectedOptionId { get; set; }

        // open text üçün
        public string? AnswerText { get; set; }

        // YENI
        public List<int> SelectedOptionIds { get; set; } = new(); // multiple choice üçün
    }
}
