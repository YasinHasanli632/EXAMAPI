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

        public ICollection<TeacherClassRoom> TeacherClassRooms { get; set; } = new List<TeacherClassRoom>();
    }
}
