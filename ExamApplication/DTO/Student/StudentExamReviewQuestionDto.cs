using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentExamReviewQuestionDto
    {
        public int Id { get; set; }

        public int ExamId { get; set; }

        public int QuestionNo { get; set; }

        public string Type { get; set; } = null!;

        public string QuestionText { get; set; } = null!;

        public List<StudentExamReviewOptionDto> Options { get; set; } = new();

        public string CorrectAnswerText { get; set; } = string.Empty;

        public string StudentAnswerText { get; set; } = string.Empty;

        public decimal AwardedScore { get; set; }

        public decimal MaxScore { get; set; }

        public string TeacherFeedback { get; set; } = string.Empty;
    }
}
