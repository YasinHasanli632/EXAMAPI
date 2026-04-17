using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Exam
{
    public class ExamListItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        // Filter və routing üçün class id ayrıca qaytarılır
        public int? ClassRoomId { get; set; }

        public string? ClassName { get; set; }

        // Filter üçün subject id ayrıca qaytarılır
        public int SubjectId { get; set; }

        public string SubjectName { get; set; } = null!;

        // Filter və teacher panel logic-i üçün teacher id ayrıca qaytarılır
        public int TeacherId { get; set; }

        public string TeacherName { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Status { get; set; } = null!;

        public int TotalQuestionCount { get; set; }

        public bool IsPublished { get; set; }

        // YENI
        public decimal? TotalScore { get; set; }
    }
}
