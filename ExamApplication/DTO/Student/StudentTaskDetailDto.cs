using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    // YENI - SAGIRD UCUN
    public class StudentTaskDetailDto
    {
        public int Id { get; set; }

        public string TaskGroupKey { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int? SubjectId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public int? TeacherId { get; set; }

        public string TeacherName { get; set; } = string.Empty;

        public int? ClassRoomId { get; set; }

        public string ClassName { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }

        public DateTime DueDate { get; set; }

        public string Status { get; set; } = string.Empty;

        // YENI - SAGIRD UCUN
        public bool IsLate { get; set; }

        // YENI - SAGIRD UCUN
        public bool CanSubmit { get; set; }

        // YENI - SAGIRD UCUN
        public bool IsSubmitted { get; set; }

        // YENI - SAGIRD UCUN
        public bool IsReviewed { get; set; }

        public decimal Score { get; set; }

        public decimal MaxScore { get; set; }

        public string? Link { get; set; }

        public string? Note { get; set; }

        public string? SubmissionText { get; set; }

        public string? SubmissionLink { get; set; }

        public string? SubmissionFileUrl { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public string? Feedback { get; set; }

        public DateTime? CheckedAt { get; set; }
    }
}
