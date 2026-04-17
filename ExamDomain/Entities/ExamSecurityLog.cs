using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class ExamSecurityLog : AuditableEntity
    {
        public int StudentExamId { get; set; }

        public ExamSecurityEventType EventType { get; set; }

        public string? Description { get; set; }

        public DateTime OccurredAt { get; set; }

        public StudentExam StudentExam { get; set; } = null!;
    }
}
