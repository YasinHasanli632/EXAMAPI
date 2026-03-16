using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class ExamQuestion : AuditableEntity
    {
        public int ExamId { get; set; } // Bu sualın aid olduğu imtahan

        public string QuestionText { get; set; } = null!; // Sualın mətni

        public QuestionType QuestionType { get; set; } // Sualın tipi

        public int Points { get; set; } // Sualın balı

        public Exam Exam { get; set; } = null!; // Navigation property

        public ICollection<ExamOption> Options { get; set; } = new List<ExamOption>(); // Test variantları

        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>(); // Tələbələrin cavabları
    }
}
