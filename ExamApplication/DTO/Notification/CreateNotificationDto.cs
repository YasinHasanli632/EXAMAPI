using ExamDomain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Notification
{
    public class CreateNotificationDto
    {
        public int UserId { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public int Type { get; set; }

        public int Category { get; set; } // YENI

        public int Priority { get; set; } // YENI

        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        public string? ActionUrl { get; set; }

        public string? Icon { get; set; } // YENI

        public string? MetadataJson { get; set; } // YENI

        public DateTime? ExpiresAt { get; set; } // YENI
    }
}
