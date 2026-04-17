using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Student
{
    public class StudentListItemDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string StudentNumber { get; set; } = null!;

        public string? ClassName { get; set; }

        public decimal AverageScore { get; set; }

        public int ExamsCount { get; set; }

        public double AttendanceRate { get; set; }

        public string Status { get; set; } = null!;

        public string? PhotoUrl { get; set; }
    }
}
