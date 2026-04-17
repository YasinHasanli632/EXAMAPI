using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentExamQuestionDto
    {
        public int Id { get; set; }
        public int OrderNumber { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Points { get; set; }
        public string? Description { get; set; }

        // YENI
        public List<StudentExamQuestionOptionDto> Options { get; set; } = new();

        // YENI
        public StudentAnswerDto? ExistingAnswer { get; set; }
    }
}
