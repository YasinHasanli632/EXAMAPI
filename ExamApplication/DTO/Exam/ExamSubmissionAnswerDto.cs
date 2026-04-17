using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class ExamSubmissionAnswerDto
    {
        public int StudentAnswerId { get; set; }
        public int QuestionId { get; set; }
        public int QuestionNo { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public decimal MaxScore { get; set; }
        public decimal AwardedScore { get; set; }
        public bool IsReviewed { get; set; }
        public bool? IsCorrect { get; set; }
        public string? StudentAnswerText { get; set; }
        public string? CorrectAnswerText { get; set; }
        public string? TeacherFeedback { get; set; }
        public List<ExamSubmissionAnswerOptionDto> Options { get; set; } = new();
    }
}
