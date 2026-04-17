using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class CreateExamDto
    {
        public string Title { get; set; } = null!;

        public int SubjectId { get; set; }

        public int TeacherId { get; set; }

        public int? ClassRoomId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DurationMinutes { get; set; }

        public string? Description { get; set; }

        public decimal? TotalScore { get; set; }

        public decimal? ClosedQuestionScore { get; set; }

        public string? Instructions { get; set; }

        public bool IsPublished { get; set; }

        public List<ExamQuestionDto> Questions { get; set; } = new();
    }
}
