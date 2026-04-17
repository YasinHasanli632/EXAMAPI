using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class Subject : AuditableEntity
    {
        public string Name { get; set; } = null!; // Fənnin adı (Riyaziyyat, Fizika və s.)

        public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>(); // Bu fənni keçən müəllimlər
                                                                                                       // YENI
        public ICollection<ClassTeacherSubject> ClassTeacherSubjects { get; set; } = new List<ClassTeacherSubject>();
        public ICollection<Exam> Exams { get; set; } = new List<Exam>(); // Bu fənn üzrə imtahanlar

        // YENI
        // Frontend option list və admin panel üçün faydalıdır.
        public string? Code { get; set; }

        // YENI
        public bool IsActive { get; set; } = true;

        // YENI
        public string? Description { get; set; }

        // YENI
        // Frontend subject formunda weeklyHours var. Backenddə də saxlanmalıdır.
        public int WeeklyHours { get; set; } = 0;

        public ICollection<StudentTask> StudentTasks { get; set; } = new List<StudentTask>(); // YENI
    }
}
