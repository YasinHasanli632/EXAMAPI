using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentExamSessionDto
    {
        public int StudentExamId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsCompleted { get; set; }
        public decimal Score { get; set; }

        // YENI
        public bool IsReviewed { get; set; }

        // YENI
        public DateTime? SubmittedAt { get; set; }

        // YENI
        public int DurationMinutes { get; set; }
        // YENI
        public string Instructions { get; set; } = string.Empty;

        // YENI
        public string Status { get; set; } = string.Empty;

        // YENI
        public decimal TotalScore { get; set; }

        // YENI
        public int WarningCount { get; set; }

        // YENI
        public int TabSwitchCount { get; set; }

        // YENI
        public int FullScreenExitCount { get; set; }

        // YENI
        public List<StudentExamQuestionDto> Questions { get; set; } = new();
    }
}
