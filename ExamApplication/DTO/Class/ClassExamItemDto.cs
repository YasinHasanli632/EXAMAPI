using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassExamItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public int SubjectId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public string ExamDate { get; set; } = string.Empty;

        public int DurationMinutes { get; set; }

        public decimal TotalScore { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
