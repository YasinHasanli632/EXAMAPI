using ExamDomain.Common;
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
        /// <summary>
        /// Müəllimin daxil olduğu sinif əlaqələri.
        /// </summary>
        public ICollection<TeacherClassRoom> TeacherClassRooms { get; set; } = new List<TeacherClassRoom>();
    }
}
