using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Teacher.Task
{
    public class StudentTaskSubmissionDetailDto
    {
        public int StudentTaskId { get; set; }
        public int StudentId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? TaskDescription { get; set; }
        public string? TaskLink { get; set; }
        public string? TaskNote { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime DueDate { get; set; }

        public string SubmissionStatus { get; set; } = string.Empty;

        public string? SubmissionText { get; set; }
        public string? SubmissionLink { get; set; }
        public string? SubmissionFileUrl { get; set; }
        public DateTime? SubmittedAt { get; set; }

        public decimal Score { get; set; }
        public decimal MaxScore { get; set; }
        public string? Feedback { get; set; }
        public DateTime? CheckedAt { get; set; }
    }
}
