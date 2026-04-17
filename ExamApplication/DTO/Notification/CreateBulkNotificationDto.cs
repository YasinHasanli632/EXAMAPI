using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Notification
{
    public class CreateBulkNotificationDto
    {
        public List<int> UserIds { get; set; } = new();

        public string Title { get; set; } = null!;

        public string Message { get; set; } = null!;

        public int Type { get; set; }

        public int Category { get; set; }

        public int Priority { get; set; }

        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }

        public string? ActionUrl { get; set; }

        public string? Icon { get; set; }

        public string? MetadataJson { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
