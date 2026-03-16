using ExamDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class AuditLog : BaseEntity
    {
        public int UserId { get; set; } // Əməliyyatı edən istifadəçi

        public string Action { get; set; } = null!; // Görülən əməliyyat (CreateExam, UpdateResult və s.)

        public string EntityName { get; set; } = null!; // Hansı entity üzərində əməliyyat edilib

        public string EntityId { get; set; } = null!; // Həmin entity-nin Id-si

        public DateTime ActionTime { get; set; } // Əməliyyatın vaxtı

        public string? Changes { get; set; } // Dəyişikliklərin JSON formasında saxlanması

        public User User { get; set; } = null!;

    }
}
