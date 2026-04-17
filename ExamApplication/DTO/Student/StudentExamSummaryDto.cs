using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentExamSummaryDto
    {
        public int StudentExamId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = null!;
        public string SubjectName { get; set; } = null!;

        // YENI
        public string TeacherName { get; set; } = string.Empty;

        public decimal Score { get; set; }

        // YENI
        public decimal MaxScore { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        // YENI
        public string ExamType { get; set; } = "Unknown";

        // YENI
        public string? Note { get; set; }

        // YENI
        public string Status { get; set; } = string.Empty;

        // YENI
        public DateTime ExamStartTime { get; set; }

        // YENI
        public DateTime ExamEndTime { get; set; }

        // YENI
        public int DurationMinutes { get; set; }

        // YENI
        public bool IsAccessCodeReady { get; set; }

        // YENI
        public bool CanEnter { get; set; }

        // YENI
        public bool CanStart { get; set; }

        // YENI
        public bool IsMissed { get; set; }

        // YENI
        public string? AccessCode { get; set; }


        public int LateEntryToleranceMinutes { get; set; }


        public int AccessCodeActivationMinutes { get; set; }

    }
}
