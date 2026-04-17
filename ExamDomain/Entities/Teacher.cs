using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class Teacher : AuditableEntity
    {
        public int UserId { get; set; } // Müəllimin bağlı olduğu user hesabı

        public string FullName { get; set; } = null!; // Müəllimin tam adı

        public string Department { get; set; } = null!; // Müəllimin bölməsi və ya ixtisası

        public User User { get; set; } = null!; // Navigation property

        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>(); // Müəllimin keçdiyi fənlər

        public ICollection<Exam> CreatedExams { get; set; } = new List<Exam>(); // Müəllimin yaratdığı imtahanlar

      

        // YENI
        // Frontend status üçün lazımdır: Aktiv / Passiv / Məzuniyyət
        public TeacherStatus Status { get; set; } = TeacherStatus.Active;
        // YENI
        public ICollection<ClassTeacherSubject> ClassTeacherSubjects { get; set; } = new List<ClassTeacherSubject>();
        // YENI
        // Frontenddə specialization gəlir. Hazırkı Department ilə eyni məna daşıyır.
        // Köhnə Department qalır, bunu service mapping-də bir-birinə bağlayarsan.
        public string? Specialization { get; set; }

        // YENI
        // Detail səhifədə task list var. Ona görə navigation lazımdır.
        public ICollection<TeacherTask> Tasks { get; set; } = new List<TeacherTask>();
        public ICollection<StudentTask> StudentTasks { get; set; } = new List<StudentTask>(); // YENI

    }
}
