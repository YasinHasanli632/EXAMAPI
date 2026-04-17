using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentOptionDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? PhotoUrl { get; set; }

        public string? ClassName { get; set; }

        public decimal? AverageScore { get; set; }

        public string Status { get; set; } = null!;
    }
}
