using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class ExamQuestionDto
    {
        public int Id { get; set; }

        public string QuestionText { get; set; } = null!;

        public string Type { get; set; } = null!;

        public int Points { get; set; }

        public int OrderNumber { get; set; }

        public string? Description { get; set; }

        public string? SelectionMode { get; set; }

        public List<ExamOptionDto> Options { get; set; } = new();
    }
}
