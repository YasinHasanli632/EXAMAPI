using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class StudentExam : AuditableEntity
    {
        public int StudentId { get; set; } // İmtahana girən tələbə

        public int ExamId { get; set; } // İmtahan

        public DateTime StartTime { get; set; } // Tələbənin imtahana başladığı vaxt

        public DateTime? EndTime { get; set; } // İmtahanı bitirdiyi vaxt

        public decimal Score { get; set; } // Ümumi bal

        public bool IsCompleted { get; set; } // İmtahan tamamlanıb ya yox
                                              // YENI
        public DateTime? SubmittedAt { get; set; } // Student submit etdiyi an

        // YENI
        public bool IsReviewed { get; set; } // Müəllim bütün yoxlamanı bitiribmi

        // YENI
        public DateTime? ReviewedAt { get; set; } // Yekun review vaxtı

        // YENI
        public int? ReviewedByTeacherId { get; set; } // Review edən teacher id

        // YENI
        public decimal AutoGradedScore { get; set; } // Closed suallardan gələn bal

        // YENI
        public decimal ManualGradedScore { get; set; } // Open suallardan verilən bal
        public Student Student { get; set; } = null!; // Navigation property

        public Exam Exam { get; set; } = null!; // Navigation property
                                                // YENI
        public StudentExamStatus Status { get; set; } = StudentExamStatus.Pending;

        // YENI
        public bool IsLocked { get; set; } = false;

        // YENI
        public int WarningCount { get; set; } = 0;

        // YENI
        public int TabSwitchCount { get; set; } = 0;

        // YENI
        public int FullScreenExitCount { get; set; } = 0;
        public bool IsAutoSubmitted { get; set; } = false;
        // YENI
        public DateTime? LastActivityAt { get; set; }
        public ICollection<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>(); // Tələbənin cavabları
    }
}
