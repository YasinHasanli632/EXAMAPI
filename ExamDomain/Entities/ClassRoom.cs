using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class ClassRoom : AuditableEntity
    {
        public string Name { get; set; } = null!; // Sinif adı (məs: 9A, 10B)

        public int Grade { get; set; } // Sinif səviyyəsi (9,10,11 və s.)

        public ICollection<StudentClass> StudentClasses { get; set; } = new List<StudentClass>(); // Bu sinifdə olan tələbələr

        public ICollection<ClassTeacherSubject> ClassTeacherSubjects { get; set; } = new List<ClassTeacherSubject>();


        // YENI
        // Frontend list/detail üçün sinifin aktiv/passiv kimi idarəsi gərəkə bilər.
        public bool IsActive { get; set; } = true;

        // YENI
        // İstəyə bağlıdır, otaq və ya kabinet kimi detail üçün rahatdır.
        public string? Room { get; set; }

        // YENI
        public string? Description { get; set; }
        // YENI
        // Frontend modelində birbaşa var
        public string AcademicYear { get; set; } = string.Empty;

        // YENI
        // Frontend create/detail üçün lazımdır
        public int MaxStudentCount { get; set; } = 30;

        // YENI
        // Class detail-də exam list çıxarmaq üçün
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();

        // YENI
        // Attendance modulu üçün indidən relation hazır olsun
        public ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();

        // YENI
        // Sinif üzrə task idarəsi üçün
        public ICollection<StudentTask> StudentTasks { get; set; } = new List<StudentTask>();
    }
}
