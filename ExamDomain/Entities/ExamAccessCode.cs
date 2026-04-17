using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class ExamAccessCode : AuditableEntity
    {
        public int ExamId { get; set; } // İmtahan Id

        public int StudentId { get; set; } // Bu kod hansı tələbə üçündür

        public string AccessCode { get; set; } = null!; // İmtahana giriş kodu

        public bool IsUsed { get; set; } // Kod istifadə olunub ya yox

        public DateTime ExpireAt { get; set; } // Kodun bitmə vaxtı
                                               // YENI
        public DateTime GeneratedAt { get; set; }

        // YENI
        public DateTime? UsedAt { get; set; }
        public Exam Exam { get; set; } = null!; // Kodun bağlı olduğu imtahan
        public Student Student { get; set; } = null!; // Kodun bağlı olduğu tələbə

    }
}
