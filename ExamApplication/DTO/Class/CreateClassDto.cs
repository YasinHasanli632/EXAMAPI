using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class CreateClassDto
    {
        public string Name { get; set; } = null!;

        public string AcademicYear { get; set; } = string.Empty;

        public string Room { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Aktiv";

        public int MaxStudentCount { get; set; } = 30;

        public List<int> SubjectIds { get; set; } = new();

        public List<ClassTeacherAssignmentDto> TeacherAssignments { get; set; } = new();

        public List<int> StudentIds { get; set; } = new();
    }
}
