using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class TeacherSubject : BaseEntity
    {
        public int TeacherId { get; set; } // Müəllimin Id-si

        public int SubjectId { get; set; } // Fənnin Id-si

        public Teacher Teacher { get; set; } = null!; // Navigation property

        public Subject Subject { get; set; } = null!; // Navigation property
    }
}
