using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Notification
{
    public class NotificationListItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string Category { get; set; } = null!; // YENI

        public string Priority { get; set; } = null!; // YENI

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ReadAt { get; set; } // YENI

        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        public string? ActionUrl { get; set; }

        public string? Icon { get; set; } // YENI
    }
}
