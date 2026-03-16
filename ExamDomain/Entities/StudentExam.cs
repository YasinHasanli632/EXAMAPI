using ExamDomain.Common;
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

        public Student Student { get; set; } = null!; // Navigation property

        public Exam Exam { get; set; } = null!; // Navigation property

        public ICollection<StudentAnswer> Answers { get; set; } = new List<StudentAnswer>(); // Tələbənin cavabları
    }
}
