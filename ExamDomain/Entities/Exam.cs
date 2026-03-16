using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class Exam : AuditableEntity
    {
        public string Title { get; set; } = null!; // İmtahanın adı

        public int SubjectId { get; set; } // İmtahanın aid olduğu fənn

        public int TeacherId { get; set; } // İmtahanı yaradan müəllim

        public DateTime StartTime { get; set; } // İmtahanın başlama vaxtı

        public DateTime EndTime { get; set; } // İmtahanın bitmə vaxtı

        public int DurationMinutes { get; set; } // İmtahanın müddəti (dəqiqə ilə)

        public ExamStatus Status { get; set; } // İmtahanın statusu

        public Subject Subject { get; set; } = null!; // Navigation property

        public Teacher Teacher { get; set; } = null!; // Navigation property

        public ICollection<ExamQuestion> Questions { get; set; } = new List<ExamQuestion>(); // İmtahanın sualları

        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>(); // Bu imtahana girən tələbələr

        public ICollection<ExamAccessCode> AccessCodes { get; set; } = new List<ExamAccessCode>();
   
    }
}
