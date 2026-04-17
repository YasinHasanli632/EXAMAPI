using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassStudentOptionDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Email { get; set; } = string.Empty;

        public string PhotoUrl { get; set; } = string.Empty;

        public string? ClassName { get; set; }

        public decimal? AverageScore { get; set; }

        public double? AttendanceRate { get; set; }

        public string Status { get; set; } = "Aktiv";
    }
}
