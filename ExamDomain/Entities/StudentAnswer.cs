using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class StudentAnswer : AuditableEntity
    {
        public int StudentExamId { get; set; } // Hansı imtahan sessiyasına aiddir

        public int ExamQuestionId { get; set; } // Hansı suala cavab verilib

        public int? SelectedOptionId { get; set; } // Single choice üçün saxlanıla bilər

        public string? AnswerText { get; set; } // Açıq sual üçün yazılan cavab

        public decimal PointsAwarded { get; set; } // Bu cavaba verilən bal

        public StudentExam StudentExam { get; set; } = null!; // Navigation property

        public ExamQuestion ExamQuestion { get; set; } = null!; // Navigation property

        public ExamOption? SelectedOption { get; set; } // Seçilmiş cavab variantı
                                                        // YENI
        public bool? IsCorrect { get; set; } // closed question auto-check nəticəsi
        // YENI
        // Multiple choice dəstəyi üçün
        public ICollection<StudentAnswerOption> SelectedOptions { get; set; } = new List<StudentAnswerOption>();
        // YENI
        public DateTime? LastSavedAt { get; set; }
        // YENI
        public bool IsReviewed { get; set; } = false;

        // YENI
        public DateTime? ReviewedAt { get; set; }

        public string? TeacherFeedback { get; set; } // YENI
    }
}
