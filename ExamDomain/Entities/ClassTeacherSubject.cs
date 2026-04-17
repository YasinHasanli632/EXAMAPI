using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    // 🔥 YENI ENTITY
    public class ClassTeacherSubject : AuditableEntity
    {
        public int ClassRoomId { get; set; }
        public ClassRoom ClassRoom { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }


        // YENI
        // Gələcəkdə müəllim dəyişəndə köhnə assignment-ləri soft-passive saxlamaq rahat olar
        public bool IsActive { get; set; } = true;
    }
}
