using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamApplication.DTO.Class
{
    public class ClassTeacherSubjectRowDto
    {
        public int SubjectId { get; set; }

        public string SubjectName { get; set; } = null!;

        public string? SubjectCode { get; set; }

        public int? AssignedTeacherId { get; set; }

        public string AssignedTeacherName { get; set; } = "Təyin edilməyib";

        public List<ClassTeacherOptionDto> AvailableTeachers { get; set; } = new();
    }
}
