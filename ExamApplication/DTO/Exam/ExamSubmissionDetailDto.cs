using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class ExamSubmissionDetailDto
    {
        public int StudentExamId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsReviewed { get; set; }
        public decimal Score { get; set; }
        public decimal AutoGradedScore { get; set; }
        public decimal ManualGradedScore { get; set; }
        public List<ExamSubmissionAnswerDto> Answers { get; set; } = new();
    }
}
