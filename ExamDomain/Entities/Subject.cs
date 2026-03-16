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

        public ICollection<Exam> Exams { get; set; } = new List<Exam>(); // Bu fənn üzrə imtahanlar
    }
}
