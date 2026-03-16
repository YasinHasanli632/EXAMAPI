using ExamDomain.Common;
using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamDomain.Entities
{
    public class Notification : AuditableEntity
    {
        public int UserId { get; set; } // Bildirişin göndərildiyi istifadəçi

        public string Title { get; set; } = null!; // Bildiriş başlığı

        public string Message { get; set; } = null!; // Bildiriş mətni

        public NotificationType Type { get; set; } // Bildiriş növü

        public bool IsRead { get; set; } // Oxunub ya yox

        public User User { get; set; } = null!; // Navigation property
    }
}
