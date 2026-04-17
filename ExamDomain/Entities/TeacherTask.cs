using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class TeacherTask : AuditableEntity
    {
        public int TeacherId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime DueDate { get; set; }

        public TeacherTaskStatus Status { get; set; } = TeacherTaskStatus.Waiting;

        public bool IsCompleted { get; set; } = false;

        public Teacher Teacher { get; set; } = null!;
    }
}
