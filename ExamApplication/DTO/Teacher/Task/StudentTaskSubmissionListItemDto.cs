using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher.Task
{
    public class StudentTaskSubmissionListItemDto
    {
        public int StudentTaskId { get; set; }
        public int StudentId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }

        public string SubmissionStatus { get; set; } = string.Empty;

        public DateTime? SubmittedAt { get; set; }

        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }

        public bool IsReviewed { get; set; }
    }
}
