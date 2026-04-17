using ExamDomain.Common;
using ExamDomain.Enum;
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

        // YENI
        // Frontend student status üçün lazımdır
        public StudentStatus Status { get; set; } = StudentStatus.Active;

        // YENI
        // Student detail səhifəsində "details" üçün lazımdır
        public string? Notes { get; set; }

        // YENI
        // Attendance modulu üçün indidən relation
        public ICollection<StudentTask> Tasks { get; set; } = new List<StudentTask>(); // YENI
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }
}
