using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassListItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string AcademicYear { get; set; } = string.Empty;

        public string Room { get; set; } = string.Empty;

        public string Status { get; set; } = "Aktiv";

        public int StudentCount { get; set; }

        public int SubjectCount { get; set; }

        public int TeacherCount { get; set; }

        public decimal AverageScore { get; set; }
    }
}
