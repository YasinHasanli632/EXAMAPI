using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class StudentClass : BaseEntity
    {
        public int StudentId { get; set; } // Tələbənin Id-si

        public int ClassRoomId { get; set; } // Sinifin Id-si

        public Student Student { get; set; } = null!; // Navigation property

        public ClassRoom ClassRoom { get; set; } = null!; // Navigation property

        // YENI
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // YENI
        public DateTime? LeftAt { get; set; }

        // YENI
        public bool IsActive { get; set; } = true;
    }
}
