using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class Student : AuditableEntity
    {
        public int UserId { get; set; } // Bu tələbənin bağlı olduğu User hesabı

        public string FullName { get; set; } = null!; // Tələbənin tam adı

        public DateTime DateOfBirth { get; set; } // Tələbənin doğum tarixi

        public string StudentNumber { get; set; } = null!; // Məktəb daxilində tələbə nömrəsi

        public User User { get; set; } = null!; // Navigation property - bağlı olduğu user

        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>(); // Tələbənin daxil olduğu siniflər

        public ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>(); // Tələbənin girdiyi imtahanlar
        public ICollection<ExamAccessCode> ExamAccessCodes { get; set; } = new List<ExamAccessCode>();
    }
}
