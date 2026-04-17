using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentAnswerDto
    {
        public int StudentAnswerId { get; set; }
        public int StudentExamId { get; set; }
        public int ExamQuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public string? AnswerText { get; set; }
        public decimal PointsAwarded { get; set; }

        // YENI
        public bool IsReviewed { get; set; }

        // YENI
        public bool? IsCorrect { get; set; }

        // YENI
        public string? TeacherFeedback { get; set; }

        // YENI
        public List<int> SelectedOptionIds { get; set; } = new();
    }
}
