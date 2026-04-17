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
        public int UserId { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public NotificationType Type { get; set; }

        public NotificationCategory Category { get; set; } // YENI

        public NotificationPriority Priority { get; set; } // YENI

        public bool IsRead { get; set; }

        public DateTime? ReadAt { get; set; } // YENI

        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        public string? ActionUrl { get; set; }

        public string? Icon { get; set; } // YENI

        public string? MetadataJson { get; set; } // YENI

        public DateTime? ExpiresAt { get; set; } // YENI

        public User User { get; set; } = null!;
    }
}
