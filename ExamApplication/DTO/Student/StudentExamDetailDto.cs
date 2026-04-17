using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentExamDetailDto
    {
        public int ExamId { get; set; }
        public int? StudentExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IsAccessCodeReady { get; set; }
        public bool CanVerifyCode { get; set; }
        public bool CanStart { get; set; }
        public bool IsCompleted { get; set; }

        // YENI
        public string? AccessCode { get; set; }



        // YENI
        public bool IsMissed { get; set; }

        // YENI
        public int AccessCodeActivationMinutes { get; set; }

        // YENI
        public int LateEntryToleranceMinutes { get; set; }

        // YENI
        public decimal Score { get; set; }

        // YENI
        public decimal? PublishedScore { get; set; }

        // YENI
        public bool RequiresManualReview { get; set; }

        // YENI
        public bool CanShowScoreImmediately { get; set; }

        // YENI
        public bool IsResultAutoPublished { get; set; }

        // YENI
        public bool HasOpenQuestions { get; set; }

        // YENI
        public string ResultMessage { get; set; } = string.Empty;

    }
}
