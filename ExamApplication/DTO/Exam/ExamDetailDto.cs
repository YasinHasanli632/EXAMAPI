using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class ExamDetailDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public int SubjectId { get; set; }

        public int TeacherId { get; set; }

        public int? ClassRoomId { get; set; }

        public string? ClassName { get; set; }

        public string SubjectName { get; set; } = null!;

        public string TeacherName { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DurationMinutes { get; set; }

        public string? Description { get; set; }

        public decimal TotalScore { get; set; }

        public decimal ClosedQuestionScore { get; set; }

        public int TotalQuestionCount { get; set; }

        public int OpenQuestionCount { get; set; }

        public int ClosedQuestionCount { get; set; }

        public string? Instructions { get; set; }

        public string Status { get; set; } = null!;

        public bool IsPublished { get; set; }

        public List<ExamQuestionDto> Questions { get; set; } = new();
    }
}
